using WrappedUSDTCollector.Models;

namespace WrappedUSDTCollector.Services;

public interface ISyncService
{
    void AddCandleToStorage(Candle candle);
    void AddCandleToStorage(IEnumerable<Candle> candles);
    IEnumerable<Candle> GetLatestCandles();
}

public class SyncService : ISyncService
{
    private readonly Dictionary<DateTime, List<Candle>> _candles = new ();
    private DateTime LastUpdate { get; set; }


    public SyncService()
    {
        var now = DateTime.UtcNow - TimeSpan.FromMinutes(2);
        LastUpdate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        
    }
    
    public void AddCandleToStorage(Candle candle)
    {
        if (candle.Timestamp < LastUpdate) return;
        var res = _candles.TryGetValue(candle.Timestamp, out List<Candle>? candles);
        if (res && candles is not null && candles.Any(x=>x.Source==candle.Source))
        {
            _candles[candle.Timestamp].Remove(candles.First(x => x.Source == candle.Source));
            _candles[candle.Timestamp].Add(candle);
            
        }
        else if (candles is null) _candles[candle.Timestamp] = [candle];
        else if (res) _candles[candle.Timestamp].Add(candle);
        
    }

    public void AddCandleToStorage(IEnumerable<Candle> candles)
    {
        foreach (var candle in candles) AddCandleToStorage(candle);
    }
    

    public IEnumerable<Candle> GetLatestCandles()
    {
        LastUpdate+= TimeSpan.FromMinutes(1);
        return _candles[LastUpdate];
    } 

   
    
    
    
}