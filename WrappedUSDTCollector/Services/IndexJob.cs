using Quartz;
using WrappedUSDTCollector.Dto;
using WrappedUSDTCollector.Models;

namespace WrappedUSDTCollector.Services;

public class IndexJob(IIndexService indexService, IYahooService yahooService, IExmoService exmoService, ISyncService syncService, ILogger<IndexJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var startTime = DateTime.UtcNow;
        Task<IEnumerable<Candle>>[] tasks = [yahooService.GetCandlesAsync("USDT-RUB", startTime-TimeSpan.FromMinutes(5), startTime, context.CancellationToken), 
        exmoService.GetCandlesAsync("USDT_RUB", startTime-TimeSpan.FromMinutes(5), startTime, context.CancellationToken)];
        
        await Task.WhenAll(tasks);
        
        logger.LogInformation("Got candles");
        
        foreach (var task in tasks) syncService.AddCandleToStorage(task.Result);
        
        var candles = syncService.GetLatestCandles();

        var result = await indexService
            .SendDataAsync([new CandleDto(candles.Average(y=>y.Close), candles.First().Timestamp)], context.CancellationToken);

        if (!result) throw new Exception("Failed to send data to index");

        
        
        logger.LogInformation("Successfully sent data to index");

    }
}