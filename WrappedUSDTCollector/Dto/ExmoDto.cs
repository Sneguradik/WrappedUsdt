namespace WrappedUSDTCollector.Dto;

public class ExmoCandleResponse
{
    public List<ExmoCandleDto> Candles { get; set; }
}

public class ExmoCandleDto
{
    public long t { get; set; }
    public double o { get; set; }
    public double c { get; set; }
    public double h { get; set; }
    public double l { get; set; }
    public double v { get; set; }
}