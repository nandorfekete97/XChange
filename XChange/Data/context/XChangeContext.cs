using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XChange.Data.Entities;

namespace XChange.Data.context;

public class XChangeContext : DbContext
{
    public XChangeContext(DbContextOptions<XChangeContext> options) : base(options) {}

    public DbSet<BookKeepingEntity> BookKeepings { get; set; }
    public DbSet<CompanyExchangeFundEntity> CompanyExchangeFunds { get; set; }
    public DbSet<CurrencyEntity> Currencies { get; set; }
    public DbSet<CurrencyRateEntity> CurrencyRates { get; set; }
    public DbSet<ExchangeInfoEntity> ExchangeInfos { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserFundEntity> UserFunds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost,1433;Database=XChange;User Id=sa;Password=WeWhoWrestleWithGod33$;Encrypt=false;");
        }
    }
}
