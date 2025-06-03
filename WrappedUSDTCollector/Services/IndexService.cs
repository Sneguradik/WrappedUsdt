using System.Text.Json;
using WrappedUSDTCollector.Dto;

namespace WrappedUSDTCollector.Services;

public interface IIndexService
{
    Task<bool> SendDataAsync(IEnumerable<CandleDto> content, CancellationToken cancellationToken = default);
}

public class IndexService(HttpClient httpClient, ILogger<IndexService> logger) : IIndexService
{
    public const string BaseUrl = "https://indexapi.spbexchange.ru";
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public async Task<bool> SendDataAsync(IEnumerable<CandleDto> content, CancellationToken cancellationToken = default)
    {
        const string url = "/indexes/v1/publish?index_code=IPSC";
        logger.LogInformation($"Sending candles from {content.Min(x=>x.Timestamp)} to {content.Max(x=>x.Timestamp)}");
        var response = await httpClient.PostAsync(
            url, new StringContent(JsonSerializer.Serialize(content, _jsonSerializerOptions)), cancellationToken);
        return response.IsSuccessStatusCode;
    }
}

public class MockIndexService(HttpClient httpClient) : IIndexService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public Task<bool> SendDataAsync(IEnumerable<CandleDto> content, CancellationToken cancellationToken = default)
    {
        var contentJson = JsonSerializer.Serialize(content, _jsonSerializerOptions);
        Console.WriteLine(contentJson);
        return Task.FromResult(true);
    }
    
}