using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using XChange.Data.Entities;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.User;
using XChange.Data.Repositories.UserFunds;
using XChange.Models;
using XChange.Services;

namespace XChange.Tests.Services;

[TestFixture]
[TestOf(typeof(UserService))]
public class UserServiceTest
{
    private Mock<IUserRepository> _userRepoMock;
    private Mock<IUserFundsRepository> _userFundsRepoMock;
    private Mock<ICurrencyRepository> _currencyRepoMock;
    private UserService _userService;

    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _userFundsRepoMock = new Mock<IUserFundsRepository>();
        _currencyRepoMock = new Mock<ICurrencyRepository>();
        _userService = new UserService(_userRepoMock.Object, _userFundsRepoMock.Object, _currencyRepoMock.Object);
        var e = "";
    }

    [Test]
    public void Test_Convert_UserFundEntity_To_Model()
    {
        // arrange
        CurrencyModel currencyModel = new CurrencyModel(1, "Forint", "HUF");
        UserFundEntity userFundEntity = new UserFundEntity(1, currencyModel.Id, 50, 130)
        {
            Id = 1
        };

        UserFundModel expectedUserFundModel = new UserFundModel(
            userFundEntity.Id, 
            UserId:userFundEntity.UserId, 
            currencyModel,
            userFundEntity.Pending, 
            userFundEntity.Disposable);
        
        // act
        var result = _userService.ConvertUserFundEntityToModel(userFundEntity, currencyModel);
        
        // assert 
        Assert.That(result.Pending, Is.EqualTo(expectedUserFundModel.Pending));
        Assert.That(result.Id, Is.EqualTo(expectedUserFundModel.Id));
        Assert.That(result.CurrencyModel, Is.EqualTo(expectedUserFundModel.CurrencyModel));
        Assert.That(result.Disposable, Is.EqualTo(expectedUserFundModel.Disposable));
    }

    [Test]
    public void Test_Convert_CurrencyEntities_To_Models()
    {
        // arrange
        CurrencyEntity currencyEntity1 = new CurrencyEntity { Name = "Forint", ShortName = "HUF" };
        CurrencyEntity currencyEntity2 = new CurrencyEntity { Name = "American Dollar", ShortName = "USD" };
        
        List<CurrencyEntity> currencyEntities = new List<CurrencyEntity>
        {
            currencyEntity1, currencyEntity2
        };

        List<CurrencyModel> expectedCurrencyModels = new List<CurrencyModel>
        {
            new CurrencyModel(currencyEntity1.Id, currencyEntity1.Name, currencyEntity1.ShortName),
            new CurrencyModel(currencyEntity2.Id, currencyEntity2.Name, currencyEntity2.ShortName)
        };

        // act
        var result = _userService.ConvertCurrencyEntitiesToModels(currencyEntities);

        // assert
        Assert.That(result, Is.EquivalentTo(expectedCurrencyModels));
    }
}