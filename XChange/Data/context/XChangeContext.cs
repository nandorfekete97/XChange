using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XChange.Data.Entities;
using XChange.Models;

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<CompanyExchangeFundEntity>().HasData(
            new CompanyExchangeFundEntity { Id = 1, Balance = 500000 }
        );

        builder.Entity<CurrencyEntity>().HasData(
            new CurrencyEntity { Id = 1, Name = "Forint", ShortName = "HUF" },
            new CurrencyEntity { Id = 2, Name = "Dollar", ShortName = "USD" },
            new CurrencyEntity { Id = 3, Name = "Euro", ShortName = "EUR" }
        );

        builder.Entity<UserEntity>().HasData(
            new UserEntity { Id = 1, FirstName = "Kobe", LastName = "Bryant" },
            new UserEntity { Id = 2, FirstName = "Jason", LastName = "Kidd" },
            new UserEntity { Id = 3, FirstName = "Lebron", LastName = "James" }
        );

        builder.Entity<UserFundEntity>().HasData(
            new UserFundEntity { Id = 1, UserId = 1, CurrencyId = 3, Disposable = 500, Pending = 0 }
        );
    }
}
