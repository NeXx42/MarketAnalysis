using MarketAnalysis.Core.Models;

public class SchedularService
{
    private readonly IPriceFetcher _priceFetcher;
    private readonly IMarketAnalysisRepository _marketAnalysisRepository;

    public SchedularService(IPriceFetcher priceFetcher, IMarketAnalysisRepository marketAnalysisRepository)
    {
        _priceFetcher = priceFetcher;
        _marketAnalysisRepository = marketAnalysisRepository;
    }

    public async Task ImportFull()
    {
        AssetPrice[]? prices = await _priceFetcher.FetchFullHistory("IBM");

        if (prices != null)
        {
            await _marketAnalysisRepository.SaveAssetPrices(prices, AssetType.Gold, true);
        }
    }

    public async Task ImportDelta()
    {

    }
}