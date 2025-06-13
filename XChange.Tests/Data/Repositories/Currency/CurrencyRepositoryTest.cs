using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using XChange.Data.context;
using XChange.Data.Entities;
using XChange.Data.Repositories.BookKeeping;
using XChange.Data.Repositories.Currency;

namespace XChange.Tests.Data.Repositories.Currency;

[TestFixture]
[TestOf(typeof(CurrencyRepository))]
public class CurrencyRepositoryTest
{
    private XChangeContext _dbContext;
    private CurrencyRepository _repository;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<XChangeContext>()
            .UseInMemoryDatabase(databaseName: "XChangeTestDb")
            .Options;

        _dbContext = new XChangeContext(options);

        _repository = new (_dbContext);
    }
    
    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task GetById_ReturnsCorrectCurrency()
    {
        CurrencyEntity currencyEntity1 = new CurrencyEntity { Name = "Forint", ShortName = "HUF" };
        CurrencyEntity currencyEntity2 = new CurrencyEntity { Name = "Euro", ShortName = "EUR" };
        
        List<CurrencyEntity> currencyEntities = new List<CurrencyEntity> { currencyEntity1, currencyEntity2 };

        await _dbContext.Currencies.AddRangeAsync(currencyEntities);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetById(currencyEntity2.Id);
        
        CompareTwoCurrencyEntities(result, currencyEntity2);
    }

    [Test]
    public async Task GetByIds_SuccessfullyReturnsCorrectCurrencies()
    {
        CurrencyEntity currencyEntity1 = new CurrencyEntity { Name = "Forint", ShortName = "HUF" };
        CurrencyEntity currencyEntity2 = new CurrencyEntity { Name = "Euro", ShortName = "EUR" };
        List<CurrencyEntity> currencyEntities = new List<CurrencyEntity> { currencyEntity1, currencyEntity2 };

        await _dbContext.Currencies.AddRangeAsync(currencyEntities);
        await _dbContext.SaveChangesAsync();

        List<int> currencyIds = new List<int>
        {
            currencyEntity1.Id, currencyEntity2.Id
        };
        
        var result = await _repository.GetByIds(currencyIds);

        Assert.That(result, Is.EquivalentTo(currencyEntities));
    }

    [Test]
    public async Task Create_SuccessfullyAddsCurrency()
    {
        CurrencyEntity currencyEntity = new CurrencyEntity { Name = "Forint", ShortName = "HUF" }; 
        
        await _repository.Create(currencyEntity);

        var result = await _repository.GetById(currencyEntity.Id);
        
        CompareTwoCurrencyEntities(result, currencyEntity);
    }

    [Test]
    public async Task GetAll_SuccessfullyReturnsAllCurrencies()
    {
        CurrencyEntity currencyEntity1 = new CurrencyEntity { Name = "Forint", ShortName = "HUF" };
        CurrencyEntity currencyEntity2 = new CurrencyEntity { Name = "Euro", ShortName = "EUR" };
        List<CurrencyEntity> currencyEntities = new List<CurrencyEntity> { currencyEntity1, currencyEntity2 };
        
        await _dbContext.Currencies.AddRangeAsync(currencyEntities);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetAll();
        
        Assert.That(result, Is.EquivalentTo(currencyEntities));
    }

    [Test]
    public async Task DeleteById_SuccessfullyDeletesCurrency()
    {
        CurrencyEntity currencyEntity1 = new CurrencyEntity { Name = "Forint", ShortName = "HUF" };
        CurrencyEntity currencyEntity2 = new CurrencyEntity { Name = "Euro", ShortName = "EUR" };
        List<CurrencyEntity> currencyEntities = new List<CurrencyEntity> { currencyEntity1, currencyEntity2 };

        await _dbContext.Currencies.AddRangeAsync(currencyEntities);
        await _dbContext.SaveChangesAsync();
        
        List<CurrencyEntity> currenciesBeforeDelete = await _repository.GetAll();

        Assert.That(currenciesBeforeDelete, Is.EquivalentTo(currencyEntities));

        await _repository.DeleteById(currencyEntity1.Id);

        var currenciesAfterDelete = await _repository.GetAll();
        Assert.That(!currenciesAfterDelete.Contains(currencyEntity1));
        Assert.That(currenciesAfterDelete.Contains(currencyEntity2));
    }
    
    private void CompareTwoCurrencyEntities(CurrencyEntity result, CurrencyEntity expected)
    {
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(expected.Name));
        Assert.That(result.ShortName, Is.EqualTo(expected.ShortName));
    }
}