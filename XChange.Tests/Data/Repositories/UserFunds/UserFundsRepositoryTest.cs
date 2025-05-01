using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using XChange.Data.context;
using XChange.Data.Entities;
using XChange.Data.Repositories.UserFunds;

namespace XChange.Tests.Data.Repositories.UserFunds;

[TestFixture]
[TestOf(typeof(UserFundsRepository))]
public class UserFundsRepositoryTest
{
    private XChangeContext _dbContext;
    private UserFundsRepository _repository;

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
        UserFundEntity userFundEntity = new UserFundEntity
        {
            CurrencyId = 1, Disposable = 100, Pending = 0, UserId = 1
        };

        await _dbContext.UserFunds.AddAsync(userFundEntity);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetById(userFundEntity.Id);

        CompareTwoUserFundEntities(result, userFundEntity);
    }

    [Test]
    public async Task GetById_FailsWithWrongId()
    {
        UserFundEntity userFundEntity = new UserFundEntity
        {
            CurrencyId = 1, Disposable = 100, Pending = 0, UserId = 1
        };

        await _dbContext.UserFunds.AddAsync(userFundEntity);
        await _dbContext.SaveChangesAsync();

        int wrongId = -1;

        var result = await _repository.GetById(wrongId);
        
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByUserId_SuccessfullyReturnsEntities()
    {
        int userId = 123;
        
        UserFundEntity userFundEntity1 = new UserFundEntity
        {
            CurrencyId = 1, Disposable = 100, Pending = 0, UserId = userId
        };

        int anotherUserId = 99;
        UserFundEntity userFundEntity2 = new UserFundEntity
        {
            CurrencyId = 1, Disposable = 100, Pending = 0, UserId = anotherUserId
        };
        
        UserFundEntity userFundEntity3 = new UserFundEntity
        {
            CurrencyId = 1, Disposable = 100, Pending = 0, UserId = userId
        };

        List<UserFundEntity> allUserFundEntities = new List<UserFundEntity>
        {
            userFundEntity1, userFundEntity2, userFundEntity3
        };
        
        await _dbContext.UserFunds.AddRangeAsync(allUserFundEntities);
        await _dbContext.SaveChangesAsync();

        List<UserFundEntity> expected = new List<UserFundEntity>
        {
            userFundEntity1, userFundEntity3
        };
        
        var result = await _repository.GetByUserId(userId);
        
        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public async Task GetByUserId_FailsWithWrongId()
    {
        UserFundEntity userFundEntity = new UserFundEntity
        {
            CurrencyId = 1, Disposable = 100, Pending = 0, UserId = 1
        };

        await _dbContext.UserFunds.AddAsync(userFundEntity);
        await _dbContext.SaveChangesAsync();

        int wrongId = -1;

        var result = await _repository.GetByUserId(wrongId);
        
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Create_SuccessfullyCreatesEntity()
    {
        UserFundEntity userFundEntity = new UserFundEntity
        {
            CurrencyId = 1, 
            Disposable = 100, 
            Pending = 0, 
            UserId = 1
        };

        await _repository.Create(userFundEntity);

        var result = await _repository.GetById(userFundEntity.Id);
        
        CompareTwoUserFundEntities(result, userFundEntity);
    }

    [Test]
    public async Task Update_SuccessfullyUpdatesEntity()
    {
        UserFundEntity userFundEntity = new UserFundEntity
        {
            CurrencyId = 1, 
            Disposable = 100, 
            Pending = 0, 
            UserId = 1
        };
        
        await _dbContext.AddAsync(userFundEntity);
        await _dbContext.SaveChangesAsync();

        UserFundEntity userFundToUpdate = await _repository.GetById(userFundEntity.Id);

        userFundToUpdate.Disposable = 80;
        userFundToUpdate.Pending = 20;
        await _repository.Update(userFundToUpdate);

        var result = await _repository.GetById(userFundToUpdate.Id);
        
        CompareTwoUserFundEntities(result, userFundToUpdate);
    }

    [Test]
    public async Task Delete_SuccessfullyDeletesEntity()
    {
        UserFundEntity userFundEntity = new UserFundEntity
        {
            CurrencyId = 1, 
            Disposable = 100, 
            Pending = 0, 
            UserId = 1
        };
        
        await _dbContext.AddAsync(userFundEntity);
        await _dbContext.SaveChangesAsync();

        UserFundEntity userFundAquired = await _repository.GetById(userFundEntity.Id);
        CompareTwoUserFundEntities(userFundAquired, userFundEntity);

        var result = await _repository.DeleteById(userFundAquired.Id);

        UserFundEntity expectedToBeNull = await _repository.GetById(userFundAquired.Id);
        
        Assert.That(result, Is.True);
        Assert.That(expectedToBeNull, Is.Null);
    }

    private void CompareTwoUserFundEntities(UserFundEntity result, UserFundEntity expected)
    {
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);
        Assert.That(result.CurrencyId, Is.EqualTo(expected.CurrencyId));
        Assert.That(result.Disposable, Is.EqualTo(expected.Disposable));
        Assert.That(result.Pending, Is.EqualTo(expected.Pending));
        Assert.That(result.UserId, Is.EqualTo(expected.UserId));
    }
}