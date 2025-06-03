using System.Text.Json.Serialization;

namespace WrappedUSDTCollector.Dto;

public class Adjclose
{
    [JsonPropertyName("adjclose")]
    public List<double> Value { get; set; }
}

public class Chart
{
    public List<Result> Result { get; set; }
    public object Error { get; set; }
}

public class CurrentTradingPeriod
{
    public Pre Pre { get; set; }
    public Regular Regular { get; set; }
    public Post Post { get; set; }
}

public class Indicators
{
    public List<Quote> Quote { get; set; }
    public List<Adjclose> Adjclose { get; set; }
}

public class Meta
{
    public string Currency { get; set; }
    public string Symbol { get; set; }
    public string ExchangeName { get; set; }
    public string FullExchangeName { get; set; }
    public string InstrumentType { get; set; }
    public long FirstTradeDate { get; set; }
    public long RegularMarketTime { get; set; }
    public bool HasPrePostMarketData { get; set; }
    public int Gmtoffset { get; set; }
    public string Timezone { get; set; }
    public string ExchangeTimezoneName { get; set; }
    public double RegularMarketPrice { get; set; }
    public double FiftyTwoWeekHigh { get; set; }
    public double FiftyTwoWeekLow { get; set; }
    public double RegularMarketDayHigh { get; set; }
    public double RegularMarketDayLow { get; set; }
    public long RegularMarketVolume { get; set; }
    public string LongName { get; set; }
    public string ShortName { get; set; }
    public double ChartPreviousClose { get; set; }
    public int PriceHint { get; set; }
    public CurrentTradingPeriod CurrentTradingPeriod { get; set; }
    public string DataGranularity { get; set; }
    public string Range { get; set; }
    public List<string> ValidRanges { get; set; }
}

public class Post
{
    public string Timezone { get; set; }
    public long Start { get; set; }
    public long End { get; set; }
    public long Gmtoffset { get; set; }
}

public class Pre
{
    public string Timezone { get; set; }
    public long Start { get; set; }
    public long End { get; set; }
    public long Gmtoffset { get; set; }
}

public class Quote
{
    public List<long?> Volume { get; set; }
    public List<double?> Low { get; set; }
    public List<double?> Close { get; set; }
    public List<double?> Open { get; set; }
    public List<double?> High { get; set; }
}

public class Regular
{
    public string Timezone { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public int Gmtoffset { get; set; }
}

public class Result
{
    public Meta Meta { get; set; }
    public List<long> Timestamp { get; set; }
    public Indicators Indicators { get; set; }
}

public class YahooChartDto
{
    public Chart Chart { get; set; }
}