using System.Text.Json.Serialization;

namespace TransactAPI.Models;

#pragma warning disable CS8618 // Data MOdel Only
public class ExchangeRateAPIResponseModel
{
    public List<ExchangeRateData> Data { get; set; }
    public MetaData Meta { get; set; }
    public LinksData Links { get; set; }
}


public class ExchangeRateData
{
    [JsonPropertyName("country_currency_desc")]
    public string CountryCurrencyDesc { get; set; }
    [JsonPropertyName("exchange_rate")]
    public string ExchangeRate { get; set; }
    [JsonPropertyName("record_date")]
    public string RecordDate { get; set; }
}

public class MetaData
{
    public int Count { get; set; }
    public Dictionary<string, string> Labels { get; set; }
    public Dictionary<string, string> DataTypes { get; set; }
    public Dictionary<string, string> DataFormats { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class LinksData
{
    public string Self { get; set; }
    public string First { get; set; }
    public string Prev { get; set; }
    public string Next { get; set; }
    public string Last { get; set; }
}
#pragma warning restore CS8618 // Data MOdel Only

