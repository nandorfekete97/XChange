using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XChange.Data.Entities;

namespace XChange.Data.context;

public class XChangeContext(DbContextOptions<DbContext> options) : DbContext
{
    public DbSet<BookKeepingEntity> BookKeepings { get; set; }
    public DbSet<CompanyExchangeFundEntity> CompanyExchangeFunds { get; set; }
    public DbSet<CurrencyEntity> Currencies { get; set; }
    public DbSet<CurrencyRateEntity> CurrencyRates { get; set; }
    public DbSet<ExchangeInfoEntity> ExchangeInfos { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserFundEntity> UserFunds { get; set; }
}