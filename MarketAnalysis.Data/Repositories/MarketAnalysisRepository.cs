using MarketAnalysis.Core.Models;
using MarketAnalysis.Data.Context;
using Microsoft.EntityFrameworkCore;

public class MarketAnalysisRepository : IMarketAnalysisRepository
{
    private readonly MarketAnalysisDbContext _db;

    public MarketAnalysisRepository(MarketAnalysisDbContext db)
    {
        _db = db;
    }

    public async Task<AssetPrice[]> GetAssetPricesAsync(AssetType assetType)
    {
        return await _db.AssetPrices.Where(x => x.Asset == assetType).ToArrayAsync();
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