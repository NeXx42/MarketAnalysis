using MarketAnalysis.Core.Models;
using MarketAnalysis.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketAnalysis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GraphController : ControllerBase
{
    private readonly PriceFetcherService _priceService;
    private readonly PriceAnalysisService _analysisService;

    public GraphController(PriceFetcherService priceService, PriceAnalysisService analysisService)
    {
        _priceService = priceService;
        _analysisService = analysisService;
    }

    public struct GraphRequest
    {
        public AnalysisTrend[] trends { get; set; }
    }

    private struct GraphResponse
    {
        public AssetPrice[] datapoints { get; set; }
        public Dictionary<AnalysisTrend, object?> trends { get; set; }
    }

    [HttpPost("{assetType}")]
    public async Task<IResult> GetGraph(AssetType assetType, [FromBody] GraphRequest req)
    {
        AssetPrice[] res = await _priceService.GetAssetPrices(assetType);
        Dictionary<AnalysisTrend, object?> trends = await _analysisService.GetTrends(res, req.trends);

        return Results.Json(new GraphResponse()
        {
            datapoints = res,
            trends = trends
        });
    }
}
