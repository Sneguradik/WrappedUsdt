using System.Text.Json;
using WrappedUSDTCollector.Dto;
using WrappedUSDTCollector.Models;

namespace WrappedUSDTCollector.Services;

public interface IExmoService
{
    Task<IEnumerable<Candle>> GetCandlesAsync(string symbol, DateTime startDate, DateTime endDate, CancellationToken token = default);
}

public class ExmoService(HttpClient client, ILogger<ExmoService> logger) : IExmoService
{
    public const string BaseUrl = "https://api.exmo.com";
    public const string SourceName = "Exmo";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IEnumerable<Candle>> GetCandlesAsync(string symbol, DateTime startDate, DateTime endDate, CancellationToken token = default)
    {
        var fromUnix = new DateTimeOffset(startDate).ToUnixTimeSeconds();
        var toUnix = new DateTimeOffset(endDate).ToUnixTimeSeconds();

        var url = $"/v1.1/candles_history?symbol={symbol}&resolution=1&from={fromUnix}&to={toUnix}";

        var req = await client.GetAsync(url, token);

        logger.LogInformation($"Requested Exmo for {symbol}");

        if (!req.IsSuccessStatusCode)
        {
            logger.LogError($"Exmo request failed for {symbol}: {req.ReasonPhrase}");
            throw new Exception($"Exmo service request failed for {symbol}");
        }

        var rawContent = await req.Content.ReadAsStringAsync(token);

        var content = JsonSerializer.Deserialize<ExmoCandleResponse>(rawContent, _jsonOptions);
        if (content?.Candles == null)
        {
            Console.WriteLine(rawContent);
            throw new Exception($"Failed to deserialize Exmo response for {symbol}");
        }

        var candles = content.Candles.Select(c => new Candle
        {
            Open = c.o,
            Close = c.c,
            High = c.h,
            Low = c.l,
            Volume = c.v,
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(c.t).UtcDateTime,
            Source = SourceName
        }).ToList();

        logger.LogInformation($"Exmo successfully processed {symbol}");
        return candles;
    }
}
