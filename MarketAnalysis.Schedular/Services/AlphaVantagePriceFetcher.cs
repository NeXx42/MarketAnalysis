using System.Net.Http.Json;
using System.Text.Json.Serialization;
using MarketAnalysis.Core.Models;
using MarketAnalysis.Core.Settings;

namespace MarketAnalysis.Schedular.Services;

public class AlphaVantagePriceFetcher : IPriceFetcher
{
    private readonly AlphaVantageSettings _settings;

    public AlphaVantagePriceFetcher(AlphaVantageSettings settings)
    {
        _settings = settings;
    }

    public async Task<AssetPrice[]?> FetchFullHistory(string symbol, AssetType assetType)
    {
        IMapper? res;

        switch (symbol)
        {
            case "OIL":
                res = await GetJson<OilPricing>("BRENT", "interval=monthly");
                break;

            default:
                res = await GetJson<WeeklyResponse>("TIME_SERIES_WEEKLY", $"symbol={symbol}");
                break;
        }

        return res?.Map(assetType);
    }

    public Task<AssetPrice[]?> FetchPricingMonth(string interval, AssetType assetType)
    {
        throw new NotImplementedException();
    }

    private async Task<T?> GetJson<T>(string function, params string[] parameters)
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"{_settings.siteURL}/query?function={function}&{string.Join("&", parameters)}&apikey={_settings.apiKey}";
            HttpResponseMessage res = await client.GetAsync(url);

            if (res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Failed request to - {url}");
            }

            return await res.Content.ReadFromJsonAsync<T>();
        }
    }


    private interface IMapper
    {
        public AssetPrice[]? Map(AssetType type);
    }

    private class OilPricing : IMapper
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("interval")] public string Interval { get; set; } = string.Empty;
        [JsonPropertyName("unit")] public string Unit { get; set; } = string.Empty;

        [JsonPropertyName("data")] public Datapoint[]? Data { get; set; }

        public AssetPrice[]? Map(AssetType type)
        {
            return Data?.Select(x =>
            {
                decimal val = decimal.Parse(x.Value);

                return new AssetPrice()
                {
                    Asset = type,
                    Date = DateTime.Parse(x.Date),
                    Close = val,
                    Open = val,
                    High = val,
                    Low = val,
                };
            }).ToArray();
        }

        public class Datapoint
        {
            [JsonPropertyName("date")] public string Date { get; set; } = string.Empty;
            [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
        }
    }

    private class WeeklyResponse : IMapper
    {
        [JsonPropertyName("Meta Data")]
        public WeeklyResponse_MetaData? MetaData { get; set; }

        [JsonPropertyName("Weekly Time Series")]
        public Dictionary<string, WeeklyResponse_TimeSeries>? WeeklyTimeSeries { get; set; }

        public AssetPrice[] Map(AssetType type)
        {
            int i = 0;
            AssetPrice[] prices = new AssetPrice[WeeklyTimeSeries!.Count];

            foreach (KeyValuePair<string, WeeklyResponse.WeeklyResponse_TimeSeries> entry in WeeklyTimeSeries)
            {
                DateTime date = DateTime.Parse(entry.Key);

                prices[i++] = new AssetPrice()
                {
                    Asset = type,
                    Close = entry.Value.Close,
                    High = entry.Value.High,
                    Low = entry.Value.Low,
                    Open = entry.Value.Open,
                    Date = date
                };
            }

            return prices;
        }

        public class WeeklyResponse_MetaData
        {
            [JsonPropertyName("1. Information")] public string Information { get; set; } = string.Empty;
            [JsonPropertyName("2. Information")] public string Symbol { get; set; } = string.Empty;
            [JsonPropertyName("3. Information")] public string LastRefreshed { get; set; } = string.Empty;
            [JsonPropertyName("4. Information")] public string TimeZone { get; set; } = string.Empty;
        }

        public class WeeklyResponse_TimeSeries
        {
            [JsonPropertyName("1. open")] public decimal Open { get; set; }
            [JsonPropertyName("2. high")] public decimal High { get; set; }
            [JsonPropertyName("3. low")] public decimal Low { get; set; }
            [JsonPropertyName("4. close")] public decimal Close { get; set; }
            [JsonPropertyName("5. volume")] public decimal Volume { get; set; }
        }
    }
}
