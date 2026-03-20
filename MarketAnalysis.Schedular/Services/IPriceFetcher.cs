using MarketAnalysis.Core.Models;

public interface IPriceFetcher
{
    public Task<AssetPrice[]?> FetchPricingMonth(string interval, AssetType assetType);
    public Task<AssetPrice[]?> FetchFullHistory(string symbol, AssetType assetType);
}