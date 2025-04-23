using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using XChange.Data.context;
using XChange.Data.Entities;
using XChange.Data.Repositories.User;

namespace XChange.Tests.Data.Repositories.User;

[TestFixture]
[TestOf(typeof(UserRepository))]
public class UserRepositoryTest
{
    private XChangeContext _dbContext;
    private UserRepository _repository;

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
        UserEntity userEntity = new UserEntity
        {
            FirstName = "Nándor",
            LastName = "Fekete"
        };

        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetById(userEntity.Id);

        CompareTwoUserEntities(result, userEntity);
    }

    [Test]
    public async Task GetById_FailsWithWrongId()
    {
        int wrongId = -1;

        var result = await _repository.GetById(wrongId);
        
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByFirstName_SuccessfullyReturnsEntity()
    {
        string firstName = "Nándor";
        UserEntity userEntity = new UserEntity
        {
            FirstName = firstName,
            LastName = "Fekete"
        };

        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetByFirstName(firstName);

        CompareTwoUserEntities(result, userEntity);
    }

    [Test]
    public async Task GetByFirstName_FailsWithWrongInput()
    {
        string firstName = "Nándor";
        UserEntity userEntity = new UserEntity
        {
            FirstName = firstName,
            LastName = "Fekete"
        };

        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetByFirstName("First");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByFullName_SuccessfullyReturnsEntity()
    {
        string firstName = "Nándor";
        string lastName = "Fekete";
        UserEntity userEntity = new UserEntity
        {
            FirstName = firstName,
            LastName = lastName
        };

        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetByFullName(firstName, lastName);

        CompareTwoUserEntities(result, userEntity);
    }

    [Test]
    public async Task GetByFullName_FailsWithWrongName()
    {
        UserEntity userEntity = new UserEntity
        {
            FirstName = "Nándor",
            LastName = "Fekete"
        };

        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetByFullName("First", "Last");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Create_SuccessfullyCreatesEntity()
    {
        UserEntity user = new UserEntity
        {
            FirstName = "Nándor",
            LastName = "Fekete"
        };

        await _repository.Create(user);

        var result = await _repository.GetById(user.Id);
        
        CompareTwoUserEntities(result, user);
    }

    [Test]
    public async Task Update_SuccessfullyUpdatesEntity()
    {
        UserEntity user = new UserEntity
        {
            FirstName = "Nándor",
            LastName = "Fekete"
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        UserEntity entityToUpdate = await _repository.GetById(user.Id);

        entityToUpdate.FirstName = "Fernando";
        entityToUpdate.LastName = "Nero";
        await _repository.Update(entityToUpdate);

        var result = await _repository.GetById(entityToUpdate.Id);

        CompareTwoUserEntities(result, entityToUpdate);
    }

    [Test]
    public async Task DeleteById_SuccessfullyDeletesEntity()
    {
        UserEntity user = new UserEntity
        {
            FirstName = "Nándor",
            LastName = "Fekete"
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        UserEntity userAquired = await _repository.GetById(user.Id);
        CompareTwoUserEntities(userAquired, user);

        var result = await _repository.DeleteById(userAquired.Id);

        UserEntity expectedToBeNull = await _repository.GetById(userAquired.Id);
        
        Assert.That(result, Is.True);
        Assert.That(expectedToBeNull, Is.Null);
    }

    private void CompareTwoUserEntities(UserEntity result, UserEntity expected)
    {
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);
        Assert.That(result.FirstName, Is.EqualTo(expected.FirstName));
        Assert.That(result.LastName, Is.EqualTo(expected.LastName));
    }
}