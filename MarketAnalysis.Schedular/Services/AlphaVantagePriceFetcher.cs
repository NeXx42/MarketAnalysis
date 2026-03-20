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

    public async Task<AssetPrice[]?> FetchFullHistory(string symbol)
    {
        WeeklyResponse? res = await GetJson<WeeklyResponse>("TIME_SERIES_WEEKLY", symbol);

        if (res != null && res.WeeklyTimeSeries != null)
        {
            int i = 0;
            AssetPrice[] prices = new AssetPrice[res.WeeklyTimeSeries.Count];

            foreach (KeyValuePair<string, WeeklyResponse.WeeklyResponse_TimeSeries> entry in res.WeeklyTimeSeries)
            {
                DateTime date = DateTime.Parse(entry.Key);

                prices[i++] = new AssetPrice()
                {
                    Asset = AssetType.Gold,
                    Close = entry.Value.Close,
                    High = entry.Value.High,
                    Low = entry.Value.Low,
                    Open = entry.Value.Open,
                    Date = date
                };
            }

            return prices;
        }

        return null;
    }

    public Task<AssetPrice[]?> FetchPricingMonth(string interval)
    {
        throw new NotImplementedException();
    }

    private async Task<T?> GetJson<T>(string function, string symbol)
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"{_settings.siteURL}/query?function={function}&symbol={symbol}&apikey={_settings.apiKey}";
            HttpResponseMessage res = await client.GetAsync(url);

            if (res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Failed request to - {url}");
            }

            return await res.Content.ReadFromJsonAsync<T>();
        }
    }

    private class WeeklyResponse
    {
        [JsonPropertyName("Meta Data")]
        public WeeklyResponse_MetaData? MetaData { get; set; }

        [JsonPropertyName("Weekly Time Series")]
        public Dictionary<string, WeeklyResponse_TimeSeries>? WeeklyTimeSeries { get; set; }

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
