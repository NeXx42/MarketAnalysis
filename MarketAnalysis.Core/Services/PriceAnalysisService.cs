using MarketAnalysis.Core.Models;

public class PriceAnalysisService
{
    public async Task<Dictionary<AnalysisTrend, object?>> GetTrends(AssetPrice[] pricingData, AnalysisTrend[] trends)
    {
        Dictionary<AnalysisTrend, object?> results = new Dictionary<AnalysisTrend, object?>();

        foreach (AnalysisTrend trend in trends)
        {
            if (results.ContainsKey(trend))
                continue;

            results[trend] = await RunJob(trend, pricingData);
        }

        return results;
    }

    private async Task<object?> RunJob(AnalysisTrend trend, AssetPrice[] pricing)
    {
        try
        {
            switch (trend)
            {
                case AnalysisTrend.AMA: return await CalculateEMA(pricing);
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    private struct EMA_Response
    {
        public decimal ema10 { get; set; }
        public decimal ema50 { get; set; }
    }

    public async Task<object> CalculateEMA(AssetPrice[] prices)
    {
        decimal[] e10 = Workout(10);
        decimal[] e50 = Workout(50);

        EMA_Response[] res = new EMA_Response[prices.Length];

        for (int i = 0; i < prices.Length; i++)
            res[i] = new EMA_Response()
            {
                ema10 = e10[i],
                ema50 = e50[i],
            };

        return res;

        decimal[] Workout(int period)
        {
            if (prices.Length <= period)
                throw new Exception("Not enough datapoints");

            decimal[] ema = new decimal[prices.Length];

            decimal k = 2m / (period + 1m);
            decimal priceAverage = 0;

            for (int i = 0; i <= period; i++)
                priceAverage += prices[i].Close;

            ema[period] = priceAverage;

            for (int i = period + 1; i < prices.Length; i++)
                ema[i] = (prices[i].Close * k) + (ema[i - 1] * (1 - k));

            return ema;
        }
    }
}