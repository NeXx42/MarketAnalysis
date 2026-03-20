using MarketAnalysis.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalysis.Data.Context;

public class MarketAnalysisDbContext : DbContext
{
    public MarketAnalysisDbContext(DbContextOptions<MarketAnalysisDbContext> options) : base(options) { }

    public DbSet<AssetPrice> AssetPrices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AssetPrice>().ToTable("AssetPrices");
    }
}