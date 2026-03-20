using MarketAnalysis.Core.Models;

public interface IPriceFetcher
{
    public Task<AssetPrice[]?> FetchPricingMonth(string interval);
    public Task<AssetPrice[]?> FetchFullHistory(string symbol);
}