using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using XChange.Data.context;
using XChange.Data.Entities;
using XChange.Data.Repositories.BookKeeping;

namespace XChange.Tests.Data.Repositories.BookKeeping;

[TestFixture]
[TestOf(typeof(BookKeepingRepository))]
public class BookKeepingRepositoryTest
{
    private XChangeContext _dbContext;
    private BookKeepingRepository _repository;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<XChangeContext>()
            .UseInMemoryDatabase(databaseName: "XChangeTestDb")
            .Options;

        _dbContext = new XChangeContext(options);

        _repository = new BookKeepingRepository(_dbContext);
    }
    
    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetById_SuccessfullyReturnsBookKeepings()
    {
        var now = DateTime.Now;
        BookKeepingEntity bookKeepingEntity1 = new BookKeepingEntity(1, now);
        BookKeepingEntity bookKeepingEntity2 = new BookKeepingEntity(2, now);

        await _dbContext.BookKeepings.AddAsync(bookKeepingEntity1);
        await _dbContext.BookKeepings.AddAsync(bookKeepingEntity2);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetById(bookKeepingEntity2.Id);
        
        CompareTwoBookKeepingEntities(result, bookKeepingEntity2);
    }

    [Test]
    public async Task GetById_FailsIfProvidedIdIsInvalid()
    {
        int wrongId = -1;

        var result = await _repository.GetById(wrongId);
        
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task CreateAsync_SuccessfullyCreatesNewBookKeeping()
    {
        var now = DateTime.Now;
        BookKeepingEntity bookKeepingEntity = new BookKeepingEntity(1, now);

        await _repository.Create(bookKeepingEntity);

        var result1 = await _repository.GetById(bookKeepingEntity.Id);

        CompareTwoBookKeepingEntities(result1, bookKeepingEntity);
    }

    [Test]
    public async Task Delete_DeletesSuccessfully()
    {
        var now = DateTime.Now;
        BookKeepingEntity bookKeepingEntity1 = new BookKeepingEntity(1, now);
        BookKeepingEntity bookKeepingEntity2 = new BookKeepingEntity(2, now);

        await _dbContext.BookKeepings.AddAsync(bookKeepingEntity1);
        await _dbContext.BookKeepings.AddAsync(bookKeepingEntity2);
        await _dbContext.SaveChangesAsync();

        await _repository.DeleteById(bookKeepingEntity2.Id);

        var result = await _repository.GetById(bookKeepingEntity2.Id);
        
        var stillExists = await _repository.GetById(bookKeepingEntity1.Id);
        
        Assert.That(stillExists, Is.Not.Null);
        Assert.That(result, Is.Null);
    }

    private void CompareTwoBookKeepingEntities(BookKeepingEntity result, BookKeepingEntity expected)
    {
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);
        Assert.That(result.CreatedAt, Is.EqualTo(expected.CreatedAt).Within(TimeSpan.FromMilliseconds(10)));
        Assert.That(result.ExchangeInfoId, Is.EqualTo(expected.ExchangeInfoId));
    }
}