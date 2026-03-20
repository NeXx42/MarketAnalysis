using MarketAnalysis.Core.Models;

public interface IMarketAnalysisRepository
{
    public Task SaveAssetPrices(AssetPrice[] prices, AssetType asset, bool clearHistory);
}