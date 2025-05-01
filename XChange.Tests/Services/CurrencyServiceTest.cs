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
        // arrange
        CurrencyRateEntity currencyRateEntity = new CurrencyRateEntity(1, 410, new DateTime(2025, 3, 11));

        CurrencyModel currencyModel = new CurrencyModel(1, "Forint", "HUF");

        CurrencyRateModel expectedCurrencyRateModel = new CurrencyRateModel(currencyModel, currencyRateEntity.Rate);
        
        // act
        var result = _currencyService.ConvertCurrencyRateEntityToModel(currencyRateEntity, currencyModel);
        
        // assert 
        Assert.That(result, Is.EqualTo(expectedCurrencyRateModel));
    }

    [Test]
    public void Test_Convert_CurrencyEntity_To_Model()
    {
        // arrange
        CurrencyEntity currencyEntity = new CurrencyEntity("Forint", "HUF");

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
            new CurrencyEntity("Forint", "HUF") { Id = 1 },
            new CurrencyEntity("American Dollar", "USD") { Id = 2 },
            new CurrencyEntity("Canadian Dollar", "CAD") { Id = 3 }
        };

        // repository makes the filter for latest currency rates, only one (latest) currency rate needs to be defined for each currency
        Dictionary<int, CurrencyRateEntity> currencyIdToMostRecentCurrencyRates = new Dictionary<int, CurrencyRateEntity>()
        {
            { 1, new CurrencyRateEntity(1, 410, new DateTime(2025, 3, 11)) }, 
            { 2, new CurrencyRateEntity(2, 1.2m, new DateTime(2025, 3, 11)) }, 
            { 3, new CurrencyRateEntity(3, 1.35m, new DateTime(2025, 3, 11)) } 
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
}