using System.Text.Json;
using WrappedUSDTCollector.Dto;
using WrappedUSDTCollector.Models;

namespace WrappedUSDTCollector.Services;

public interface IInvestingService
{
    Task<IEnumerable<Candle>> GetCandlesAsync(string ticker, CancellationToken token = default);
};

public class InvestingService(HttpClient client, ILogger<InvestingService> logger) : IInvestingService
{
    public const string BaseUrl = "https://api.investing.com";
    public const string SourceName = "Investing";
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
    };
    
    public async Task<IEnumerable<Candle>> GetCandlesAsync(string ticker, CancellationToken token = default)
    {
        var url = $"/api/financialdata/{ticker}/historical/chart/?interval=PT1M&pointscount=60";
        
        var req = await client.GetAsync(url, token);
        
        logger.LogInformation($"Requested investing for {ticker}");
        
        
        if (!req.IsSuccessStatusCode) throw new Exception($"Investing failed {ticker}");
        
        var rawContent = await req.Content.ReadAsStringAsync(token);
        
        var content = JsonSerializer.Deserialize<InvestingDto>(rawContent, _jsonSerializerOptions);
        
        if (content == null) throw new Exception($"Investing failed {ticker}");
        
        logger.LogInformation($"Received investing for {ticker}");
        
        var candles = new List<Candle>();

        foreach (var candle in content.Data)
        {
            candles.Add(new Candle
            {
                Open = (double)candle[1],
                High = (double)candle[2],
                Low = (double)candle[3],
                Close = (double)candle[4],
                Timestamp = DateTimeOffset
                    .FromUnixTimeMilliseconds((long)candle[0])
                    .ToUniversalTime()
                    .DateTime,
                Volume = (double)candle[5],
                Source = SourceName
            });
        }
        return candles;
    }
}