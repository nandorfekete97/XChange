using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using XChange.Data.Entities;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.CurrencyRate;
using XChange.Models;
using XChange.Services;

namespace XChange.Tests.Services;

[TestFixture]
[TestOf(typeof(CurrencyService))]
public class CurrencyServiceTest
{
    private Mock<ICurrencyRepository> _currencyRepoMock;
    private Mock<ICurrencyRateRepository> _currencyRateRepoMock;
    private CurrencyService _currencyService;
    
    [SetUp]
    public void SetUp()
    {
        _currencyRepoMock = new Mock<ICurrencyRepository>();
        _currencyRateRepoMock = new Mock<ICurrencyRateRepository>();
        _currencyService = new CurrencyService(_currencyRepoMock.Object, _currencyRateRepoMock.Object);
    }

    [Test]
    public void Test_Convert_CurrencyRateEntity_To_Model()
    {
        CurrencyRateEntity currencyRateEntity = new CurrencyRateEntity
        {
            Id = 1,
            CurrencyId = 1,
            Rate = 410,
            Timestamp = DateTime.UtcNow,
        };
        
        CurrencyModel currencyModel = new CurrencyModel(1, "Forint", "HUF");

        CurrencyRateModel expectedCurrencyRateModel = new CurrencyRateModel(1, currencyModel, currencyRateEntity.Rate);
        
        // act
        var result = _currencyService.ConvertCurrencyRateEntityToModel(currencyRateEntity, currencyModel);
        
        // assert 
        Assert.That(result, Is.EqualTo(expectedCurrencyRateModel));
    }

    [Test]
    public void Test_Convert_CurrencyEntity_To_Model()
    {
        // arrange
        CurrencyEntity currencyEntity = new CurrencyEntity { Name = "Forint", ShortName = "HUF" };

        CurrencyModel expectedCurrencyModel = new CurrencyModel(currencyEntity.Id, currencyEntity.Name, currencyEntity.ShortName);

        // act
        var result = _currencyService.ConvertCurrencyEntityToModel(currencyEntity);
        
        // assert
        Assert.That(result, Is.EqualTo(expectedCurrencyModel));
    }

    [Test]
    public async Task Test_GetLastCurrencyRates_By_CurrencyIds()
    {
        // arrange
        List<CurrencyEntity> currencyEntities = new List<CurrencyEntity>()
        {
            new CurrencyEntity { Id = 1, Name = "Forint", ShortName = "HUF" },
            new CurrencyEntity { Id = 2, Name = "Dollar", ShortName = "USD" },
            new CurrencyEntity { Id = 3, Name = "Canadian Dollar", ShortName = "CAD" }
        };

        // repository makes the filter for latest currency rates, only one (latest) currency rate needs to be defined for each currency
        Dictionary<int, CurrencyRateEntity> currencyIdToMostRecentCurrencyRates = new Dictionary<int, CurrencyRateEntity>
        {
            { 1, new CurrencyRateEntity { CurrencyId = 1, Rate = 410m, Timestamp = new DateTime(2025, 3, 11) } },
            { 2, new CurrencyRateEntity { CurrencyId = 2, Rate = 1.2m, Timestamp = new DateTime(2025, 3, 11) } },
            { 3, new CurrencyRateEntity { CurrencyId = 3, Rate = 1.35m, Timestamp = new DateTime(2025, 3, 11) } }
        };

        _currencyRepoMock
            .Setup(repo => repo.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(currencyEntities);

        _currencyRateRepoMock
            .Setup(repo => repo.GetLastCurrencyRateByCurrencyIds(It.IsAny<List<int>>()))
            .ReturnsAsync(currencyIdToMostRecentCurrencyRates);

        // act
        var result = await _currencyService.GetLastCurrencyRatesByCurrencyIds(new List<int> { 1, 2, 3 });

        // assert
        Assert.That(result.Count, Is.EqualTo(3));

        Assert.That(result[0].Rate, Is.EqualTo(410)); 
        Assert.That(result[1].Rate, Is.EqualTo(1.2m)); 
        Assert.That(result[2].Rate, Is.EqualTo(1.35m)); 
    }

    [Test]
    public async Task GetLastCurrencyRatesByCurrencyIds_ZeroCurrencyIds_ReturnsEmptyList()
    {
        var zeroCurrencyIds = new List<int>();

        var result = await _currencyService.GetLastCurrencyRatesByCurrencyIds(zeroCurrencyIds);

        var emptyCurrencyRateModelList = new List<CurrencyRateModel>();
        
        Assert.That(result, Is.EquivalentTo(emptyCurrencyRateModelList));
    }

    [Test]
    public async Task GetLastCurrencyRatesByCurrencyIds_ZeroCurrencyEntities_ReturnsEmptyList()
    {
        var emptyCurrencyEntityList = new List<CurrencyEntity>();

        _currencyRepoMock.Setup(repository => repository.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(emptyCurrencyEntityList);

        List<int> validCurrencyIds = new List<int> { 1, 2, 3, 4, 5 };

        var emptyCurrencyRateModelList = new List<CurrencyRateModel>();
        
        var result = await _currencyService.GetLastCurrencyRatesByCurrencyIds(validCurrencyIds);
        
        Assert.That(result, Is.EquivalentTo(emptyCurrencyRateModelList));
    }

    [Test]
    public void GetLastCurrencyRatesByCurrencyIds_MissingCurrencyRate_ThrowsException()
    {
        CurrencyEntity currencyEntity1 = new CurrencyEntity { Id = 1, Name = "Forint", ShortName = "HUF" };
        CurrencyEntity currencyEntity2 = new CurrencyEntity { Id = 2, Name = "Dollar", ShortName = "USD" };

        List<CurrencyEntity> currencyEntities = new() 
            { currencyEntity1, currencyEntity2 };

        Dictionary<int, CurrencyRateEntity> incompleteRates = new()
        {
            { 1, new CurrencyRateEntity { CurrencyId = 1, Rate = 410m, Timestamp = DateTime.Now } }
        };

        _currencyRepoMock.Setup(repository => repository.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(currencyEntities);
        _currencyRateRepoMock.Setup(repository => repository.GetLastCurrencyRateByCurrencyIds(It.IsAny<List<int>>()))
            .ReturnsAsync(incompleteRates);

        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _currencyService.GetLastCurrencyRatesByCurrencyIds(new List<int> { currencyEntity1.Id, currencyEntity2.Id }));
    }

    [Test]
    public async Task GetLastCurrencyRatesByCurrencyIds_SomeInvalidCurrencyIds_ReturnsOnlyValidRates()
    {
        CurrencyEntity currencyEntity1 = new CurrencyEntity { Id = 2, ShortName = "HUF", Name = "Forint" };

        List<CurrencyEntity> currencyEntities = new() { currencyEntity1 };

        Dictionary<int, CurrencyRateEntity> currencyRates = new()
        {
            { 2, new CurrencyRateEntity { CurrencyId = currencyEntity1.Id, Rate = 1.2m, Timestamp = DateTime.Now } }
        };

        _currencyRepoMock.Setup(repository => repository.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(currencyEntities);
        _currencyRateRepoMock.Setup(repository => repository.GetLastCurrencyRateByCurrencyIds(It.IsAny<List<int>>()))
            .ReturnsAsync(currencyRates);

        int invalidCurrencyId1 = 3;
        int invalidCurrencyId2 = 4;
        
        var result = await _currencyService.GetLastCurrencyRatesByCurrencyIds(new List<int> { currencyEntity1.Id, invalidCurrencyId1, invalidCurrencyId2 });
        
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Rate, Is.EqualTo(1.2m));
    }

    [Test]
    public async Task GetLastCurrencyRatesByCurrencyIds_DuplicateInputIds_NoDuplicateResults()
    {
        var currencyEntities = new List<CurrencyEntity>
        {
            new CurrencyEntity { Id = 1, Name = "Forint", ShortName = "HUF" }
        };

        var currencyRates = new Dictionary<int, CurrencyRateEntity>
        {
            { 1, new CurrencyRateEntity { CurrencyId = 1, Rate = 410m, Timestamp = DateTime.Now } }
        };

        _currencyRepoMock.Setup(repository => repository.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(currencyEntities);
        _currencyRateRepoMock.Setup(repository => repository.GetLastCurrencyRateByCurrencyIds(It.IsAny<List<int>>()))
            .ReturnsAsync(currencyRates);

        var result = await _currencyService.GetLastCurrencyRatesByCurrencyIds(new List<int> { 1, 1, 1 });
        
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetById_ValidId_ReturnsCurrencyModel()
    {
        CurrencyEntity currencyEntity1 = new CurrencyEntity
        {
            Id = 1, Name = "Forint", ShortName = "HUF"
        };

        _currencyRepoMock.Setup(repository => repository.GetById(It.IsAny<int>())).ReturnsAsync(currencyEntity1);

        var result = await _currencyService.GetById(currencyEntity1.Id);
        
        Assert.That(result.Id, Is.EqualTo(currencyEntity1.Id));
        Assert.That(result.ShortName, Is.EqualTo(currencyEntity1.ShortName));
        Assert.That(result.Name, Is.EqualTo(currencyEntity1.Name));
    }

    [Test]
    public async Task GetById_InvalidId_ThrowsException()
    {
        int invalidId = 0;

        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _currencyService.GetById(invalidId));
        
        Assert.That(exception.Message, Is.EqualTo("ID must be positive integer."));
        _currencyRepoMock.Verify(repository => repository.GetById(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task GetByIds_ValidIds_ReturnsModels()
    {
        CurrencyEntity currencyEntity1 = new CurrencyEntity { Id = 1, Name = "Forint", ShortName = "HUF" };
        CurrencyEntity currencyEntity2 = new CurrencyEntity
        {
            Id = 2, Name = "Dollar", ShortName = "USD"
        };

        List<int> currencyIds = new List<int> { currencyEntity1.Id, currencyEntity2.Id };
        List<CurrencyEntity> currencyEntities = new List<CurrencyEntity>
        {
            currencyEntity1, currencyEntity2
        };

        _currencyRepoMock.Setup(repository => repository.GetByIds(currencyIds)).ReturnsAsync(currencyEntities);

        List<CurrencyModel> expected = new List<CurrencyModel>
        {
            new CurrencyModel(currencyEntity1.Id, currencyEntity1.Name, currencyEntity1.ShortName),
            new CurrencyModel(currencyEntity2.Id, currencyEntity2.Name, currencyEntity2.ShortName)
        };

        var result = await _currencyService.GetByIds(currencyIds);
        
        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public async Task GetByIds_InvalidId_ThrowsException()
    {
        List<int> currencyIds = new List<int>{1,2,0,4};

        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _currencyService.GetByIds(currencyIds));
        
        Assert.That(exception.Message, Is.EqualTo("All IDs must be positive integers."));
        _currencyRepoMock.Verify(repository => repository.GetByIds(It.IsAny<List<int>>()), Times.Never);
    }
    
    
}