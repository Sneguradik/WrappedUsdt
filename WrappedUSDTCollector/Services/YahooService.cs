using System.Text.Json;
using WrappedUSDTCollector.Dto;
using WrappedUSDTCollector.Models;

namespace WrappedUSDTCollector.Services;

public interface IYahooService
{
    Task<IEnumerable<Candle>> GetCandlesAsync(string ticker, DateTime startDate, DateTime endDate,
        CancellationToken token = default);
};

public class YahooService(HttpClient client, ILogger<YahooService> logger) : IYahooService
{
    public const string BaseUrl = "https://query2.finance.yahoo.com";
    public const string SourceName = "Yahoo";
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };
    
    public async Task<IEnumerable<Candle>> GetCandlesAsync(string ticker, DateTime startDate, DateTime endDate,
        CancellationToken token = default)
    {
        var startDateUnix = new DateTimeOffset(startDate).ToUnixTimeSeconds();
        var endDateUnix = new DateTimeOffset(endDate).ToUnixTimeSeconds();

        var url =
            $"/v8/finance/chart/{ticker}?period1={startDateUnix}&period2={endDateUnix}&interval=1m&includePrePost=false&lang=en-US&region=US&source=cosaic";
        
        var req = await client.GetAsync(url, token);
        
        logger.LogInformation($"Requested yahoo for {ticker}");

        if (!req.IsSuccessStatusCode)
        {
            Console.WriteLine(req.ReasonPhrase);
            throw new Exception($"Yahoo service request returned unsuccessfully for {ticker}");
        }
        var rawContent = await req.Content.ReadAsStringAsync(token);
        var content =  JsonSerializer
            .Deserialize<YahooChartDto>(rawContent, _jsonOptions);
        
        if (content is null) throw new Exception($"Error in serialization {ticker}");
        
        logger.LogInformation($"Data serialized for {ticker}");
        
        var candles = new List<Candle>();

        if (content.Chart.Result[0].Timestamp.Count != 0)
        {
            for (var i = 0; i < content.Chart.Result[0].Timestamp.Count; i++)
            {

                if(content.Chart.Result[0].Indicators.Quote[0].Close[i] is null ||
                   content.Chart.Result[0].Indicators.Quote[0].Open[i] is null ||
                   content.Chart.Result[0].Indicators.Quote[0].High[i] is null ||
                   content.Chart.Result[0].Indicators.Quote[0].Close[i] is null) continue;
                candles.Add(new Candle()
                {
                    Close = (double)content.Chart.Result[0].Indicators.Quote[0].Close[i]!,
                    Open = (double)content.Chart.Result[0].Indicators.Quote[0].Open[i]!,
                    High = (double)content.Chart.Result[0].Indicators.Quote[0].High[i]!,
                    Low = (double)content.Chart.Result[0].Indicators.Quote[0].Low[i]!,
                    Volume = (double)content.Chart.Result[0].Indicators.Quote[0].Volume[i]!,
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(content.Chart.Result[0].Timestamp[i]).UtcDateTime,
                    Source = SourceName
                });
            }
        }
        
        logger.LogInformation($"Yahoo successfully processed {ticker}");
        
        return candles;
    }
}