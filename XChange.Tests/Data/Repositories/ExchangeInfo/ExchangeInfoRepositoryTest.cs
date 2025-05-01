using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using XChange.Data.context;
using XChange.Data.Entities;
using XChange.Data.Repositories.ExchangeInfo;
using XChange.Models;

namespace XChange.Tests.Data.Repositories.ExchangeInfo;

[TestFixture]
[TestOf(typeof(ExchangeInfoRepository))]
public class ExchangeInfoRepositoryTest
{
    private XChangeContext _dbContext;
    private ExchangeInfoRepository _repository;

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
    public async Task GetById_SuccessfullyReturnsEntity()
    {
        var startedAt = DateTime.UtcNow;
        var entity = new ExchangeInfoEntity(
            startedAt,
            userId: 1,
            sourceCurrencyId: 100,
            targetCurrencyId: 200,
            sourceCurrencyAmount: 150.50m,
            status: ExchangeStatus.Verification
        );

        await _dbContext.ExchangeInfos.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetById(entity.Id);
        
        CompareTwoExchangeInfoEntities(result, entity);
    }

    [Test]
    public async Task GetById_FailsIfIdIsInvalid()
    {
        int wrongId = -1;

        ExchangeInfoEntity exchangeInfoEntity = new ExchangeInfoEntity
        {
            Id = wrongId
        };

        var result = await _repository.GetById(wrongId);
        
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Create_SuccessfullyCreatesEntity()
    {
        ExchangeInfoEntity exchangeInfoEntity = new ExchangeInfoEntity
        {
            StartedAt = DateTime.UtcNow,
            UserId = 1,
            SourceCurrencyId = 1,
            TargetCurrencyId = 2,
            SourceCurrencyAmount = 100,
            Status = ExchangeStatus.Verification,
            CurrencyRateId = 3
        };

        await _repository.Create(exchangeInfoEntity);

        var result = await _repository.GetById(exchangeInfoEntity.Id);
        
        CompareTwoExchangeInfoEntities(result, exchangeInfoEntity);
    }

    [Test]
    public async Task Update_SuccessfullyUpdatesEntity()
    {
        ExchangeInfoEntity exchangeInfoEntity = new ExchangeInfoEntity
        {
            StartedAt = DateTime.UtcNow,
            UserId = 1,
            SourceCurrencyId = 1,
            TargetCurrencyId = 2,
            SourceCurrencyAmount = 100,
            Status = ExchangeStatus.Verification,
            CurrencyRateId = 3
        };

        await _dbContext.AddAsync(exchangeInfoEntity);
        await _dbContext.SaveChangesAsync();

        ExchangeInfoEntity entityToUpdate = await _repository.GetById(exchangeInfoEntity.Id);
        
        entityToUpdate.Status = ExchangeStatus.Accepted;
        await _repository.Update(entityToUpdate);

        var result = await _repository.GetById(entityToUpdate.Id);
        
        CompareTwoExchangeInfoEntities(result, entityToUpdate);
    }
    
    [Test]
    public async Task DeleteById_SuccessfullyDeletesEntity()
    {
        ExchangeInfoEntity exchangeInfoEntity = new ExchangeInfoEntity
        {
            StartedAt = DateTime.UtcNow,
            UserId = 1,
            SourceCurrencyId = 1,
            TargetCurrencyId = 2,
            SourceCurrencyAmount = 100,
            Status = ExchangeStatus.Verification,
            CurrencyRateId = 3
        };

        await _dbContext.ExchangeInfos.AddAsync(exchangeInfoEntity);
        await _dbContext.SaveChangesAsync();

        ExchangeInfoEntity exchangeAquired = await _repository.GetById(exchangeInfoEntity.Id);
        CompareTwoExchangeInfoEntities(exchangeAquired, exchangeInfoEntity);

        var result = await _repository.DeleteById(exchangeAquired.Id);

        ExchangeInfoEntity expectedToBeNull = await _repository.GetById(exchangeAquired.Id);
        
        Assert.That(result, Is.True);
        Assert.That(expectedToBeNull, Is.Null);
    }

    private void CompareTwoExchangeInfoEntities(ExchangeInfoEntity result, ExchangeInfoEntity expected)
    {
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);
        Assert.That(result.StartedAt, Is.EqualTo(expected.StartedAt));
        Assert.That(result.Status, Is.EqualTo(expected.Status));
        Assert.That(result.UserId, Is.EqualTo(expected.UserId));
        Assert.That(result.FailedAt, Is.EqualTo(expected.FailedAt));
        Assert.That(result.CurrencyRateId, Is.EqualTo(expected.CurrencyRateId));
        Assert.That(result.SourceCurrencyAmount, Is.EqualTo(expected.SourceCurrencyAmount));
        Assert.That(result.SourceCurrencyId, Is.EqualTo(expected.SourceCurrencyId));
        Assert.That(result.TargetCurrencyId, Is.EqualTo(expected.TargetCurrencyId));
    }
    
}