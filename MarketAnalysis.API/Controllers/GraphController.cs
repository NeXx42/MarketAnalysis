using MarketAnalysis.Core.Models;
using MarketAnalysis.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketAnalysis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GraphController : ControllerBase
{
    private readonly PriceFetcherService _priceService;

    public GraphController(PriceFetcherService priceService)
    {
        _priceService = priceService;
    }

    private struct GraphResponse
    {
        public AssetPrice[] datapoints { get; set; }
    }

    [HttpGet("{assetType}")]
    public async Task<IResult> GetGraph(AssetType assetType)
    {
        AssetPrice[] res = await _priceService.GetAssetPrices(assetType);

        return Results.Json(new GraphResponse()
        {
            datapoints = res
        });
    }
}
