using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using XChange.Data.context;
using XChange.Data.Entities;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.CurrencyRate;

namespace XChange.Tests.Data.Repositories.CurrencyRate;

[TestFixture]
[TestOf(typeof(CurrencyRateRepository))]
public class CurrencyRateRepositoryTest
{
    private XChangeContext _dbContext;
    private CurrencyRateRepository _repository;

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
    public async Task GetById_SuccessfullyReturnsCurrencyRate()
    {
        CurrencyRateEntity currencyRateEntity = new CurrencyRateEntity(1, 10, DateTime.UtcNow);

        await _dbContext.CurrencyRates.AddAsync(currencyRateEntity);
        await _dbContext.SaveChangesAsync();
        
        var result = await _repository.GetById(currencyRateEntity.Id);
        
        CompareTwoCurrencyRateEntities(result, currencyRateEntity);
    }

    [Test]
    public async Task GetByCurrencyId_SuccessfullyReturnsCurrencyRate()
    {
        CurrencyEntity currencyEntity1 = new CurrencyEntity("Forint", "HUF");
        
        await _dbContext.Currencies.AddAsync(currencyEntity1);
        await _dbContext.SaveChangesAsync();
        
        DateTime now = DateTime.UtcNow;
        CurrencyRateEntity currencyRateEntity1 = new CurrencyRateEntity(currencyEntity1.Id, 10, now);

        CurrencyRateEntity currencyRateEntity2 = new CurrencyRateEntity(currencyEntity1.Id, 10, now);

        int differentId = 99;
        CurrencyRateEntity currencyRateEntity3 = new CurrencyRateEntity(differentId, 10, now);
            
        await _dbContext.CurrencyRates.AddAsync(currencyRateEntity1);
        await _dbContext.CurrencyRates.AddAsync(currencyRateEntity2);
        await _dbContext.CurrencyRates.AddAsync(currencyRateEntity3);
        await _dbContext.SaveChangesAsync();

        List<CurrencyRateEntity> expected = new List<CurrencyRateEntity>
        {
            currencyRateEntity1, currencyRateEntity2
        };
        
        var result = await _repository.GetByCurrencyId(currencyEntity1.Id);

        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public async Task GetLastCurrencyRateByCurrencyIds_ReturnsCorrectCurrenciesWithCurrencyRates()
    {
        CurrencyEntity forint = new CurrencyEntity("Forint", "HUF");
        CurrencyEntity dollar = new CurrencyEntity("Dollar", "USD");
        
        List<CurrencyEntity> currencyEntities = new List<CurrencyEntity>
        {
            forint, dollar
        };

        await _dbContext.Currencies.AddRangeAsync(currencyEntities);
        await _dbContext.SaveChangesAsync();

        DateTime today = DateTime.Today;
        
        CurrencyRateEntity forintRate1 = new CurrencyRateEntity(forint.Id, 10, today.AddDays(-1));
        CurrencyRateEntity forintRate2 = new CurrencyRateEntity(forint.Id, 10, today);
        CurrencyRateEntity dollarRate1 = new CurrencyRateEntity(dollar.Id, 10, today);
        CurrencyRateEntity dollarRate2 = new CurrencyRateEntity(dollar.Id, 10, today.AddDays(-1));
        
        List<CurrencyRateEntity> currencyRateEntities = new List<CurrencyRateEntity>
        {
            forintRate1, forintRate2, dollarRate1, dollarRate2
        };

        await _dbContext.CurrencyRates.AddRangeAsync(currencyRateEntities);
        await _dbContext.SaveChangesAsync();

        List<int> currencyIds = new List<int> { forint.Id, dollar.Id };
        
        var result = await _repository.GetLastCurrencyRateByCurrencyIds(currencyIds);
        
        Dictionary<int, CurrencyRateEntity> expected = new Dictionary<int, CurrencyRateEntity>
        {
            {forint.Id, forintRate2},
            {dollar.Id, dollarRate1}
        };

        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public async Task Create_SuccessfullyCreatesEntity()
    {
        CurrencyRateEntity currencyRateEntity = new CurrencyRateEntity(1, 1, DateTime.UtcNow);

        await _repository.Create(currencyRateEntity);

        var result = await _repository.GetById(currencyRateEntity.Id);
        
        CompareTwoCurrencyRateEntities(result, currencyRateEntity);
    }

    [Test]
    public async Task DeleteById_SuccessfullyDeletesEntity()
    {
        CurrencyRateEntity currencyRateEntity = new CurrencyRateEntity(1, 1, DateTime.UtcNow);

        await _dbContext.CurrencyRates.AddAsync(currencyRateEntity);
        await _dbContext.SaveChangesAsync();

        var result1 = await _repository.DeleteById(currencyRateEntity.Id);
        var result2 = await _repository.GetById(currencyRateEntity.Id);
        
        Assert.That(result1, Is.True);
        Assert.That(result2, Is.Null);
    }
    
    private void CompareTwoCurrencyRateEntities(CurrencyRateEntity result, CurrencyRateEntity expected)
    {
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);
        Assert.That(result.CurrencyId, Is.EqualTo(expected.CurrencyId));
        Assert.That(result.Timestamp, Is.EqualTo(expected.Timestamp));
        Assert.That(result.Rate, Is.EqualTo(expected.Rate));
    }
}