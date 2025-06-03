namespace WrappedUSDTCollector.Models;

public class Candle
{
    public string Source { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double Open { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }
    public double Volume { get; set; }
}