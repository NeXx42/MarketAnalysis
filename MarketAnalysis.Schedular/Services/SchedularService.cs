using System.Runtime.Serialization;
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
        AssetPrice[]? prices = await _priceFetcher.FetchFullHistory("OIL", AssetType.Oil);
        await SavePrices(prices, AssetType.Oil);

        prices = await _priceFetcher.FetchFullHistory("VOO", AssetType.Shares);
        await SavePrices(prices, AssetType.Shares);

        async Task SavePrices(AssetPrice[]? prices, AssetType type)
        {
            if (prices == null)
                return;

            await _marketAnalysisRepository.SaveAssetPrices(prices, type, true);
        }
    }

    public async Task ImportDelta()
    {

    }
}