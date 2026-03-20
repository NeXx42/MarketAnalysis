using MarketAnalysis.Core.Models;

namespace MarketAnalysis.Core.Services;

public class PriceFetcherService
{
    private readonly IMarketAnalysisRepository _marketAnalysisRepository;

    public PriceFetcherService(IMarketAnalysisRepository marketAnalysisRepository)
    {
        _marketAnalysisRepository = marketAnalysisRepository;
    }

    public async Task<AssetPrice[]> GetAssetPrices(AssetType type)
    {
        return await _marketAnalysisRepository.GetAssetPricesAsync(type);
    }
}
