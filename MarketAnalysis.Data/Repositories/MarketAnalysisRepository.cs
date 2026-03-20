using MarketAnalysis.Core.Models;
using MarketAnalysis.Data.Context;

public class MarketAnalysisRepository : IMarketAnalysisRepository
{
    private readonly MarketAnalysisDbContext _db;

    public MarketAnalysisRepository(MarketAnalysisDbContext db)
    {
        _db = db;
    }

    public async Task SaveAssetPrices(AssetPrice[] prices, AssetType asset, bool clearHistory)
    {
        if (clearHistory)
        {
            _db.RemoveRange(_db.AssetPrices.Where(x => x.Asset == asset));
        }

        await _db.AddRangeAsync(prices);
        await _db.SaveChangesAsync();
    }
}