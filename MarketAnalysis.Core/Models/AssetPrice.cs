namespace MarketAnalysis.Core.Models;

public class AssetPrice
{
    public int Id { get; set; }
    public AssetType Asset { get; set; }
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
}
