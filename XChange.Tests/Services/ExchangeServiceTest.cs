using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using XChange.Data.Entities;
using XChange.Data.Repositories.BookKeeping;
using XChange.Data.Repositories.CompanyExchangeFunds;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.CurrencyRate;
using XChange.Data.Repositories.ExchangeInfo;
using XChange.Data.Repositories.User;
using XChange.Data.Repositories.UserFunds;
using XChange.Models;
using XChange.Services;
using System.Linq;

namespace XChange.Tests.Services;

[TestFixture]
[TestOf(typeof(ExchangeService))]
public class ExchangeServiceTest
{
    private Mock<IUserRepository> _userRepoMock;
    private Mock<IUserFundsRepository> _userFundsRepoMock;
    private Mock<ICurrencyRepository> _currencyRepoMock;
    private Mock<ICurrencyRateRepository> _currencyRateRepoMock;
    private Mock<IExchangeInfoRepository> _exchangeInfoRepoMock;
    private Mock<ICompanyExchangeFundsRepository> _companyExchangeFundsRepoMock;
    private Mock<IBookKeepingRepository> _bookKeepingRepoMock;
    private Mock<IUserService> _userServiceMock;
    private ExchangeService _exchangeService;
    
    [SetUp]
    public void SetUp()
    {
        _userRepoMock = new Mock<IUserRepository>();
    _userFundsRepoMock = new Mock<IUserFundsRepository>();
    _currencyRepoMock = new Mock<ICurrencyRepository>();
    _currencyRateRepoMock = new Mock<ICurrencyRateRepository>();
    _exchangeInfoRepoMock = new Mock<IExchangeInfoRepository>();
    _companyExchangeFundsRepoMock = new Mock<ICompanyExchangeFundsRepository>();
    _bookKeepingRepoMock = new Mock<IBookKeepingRepository>();
    _userServiceMock = new Mock<IUserService>();
    _exchangeService = new ExchangeService(_userRepoMock.Object, _userFundsRepoMock.Object, _currencyRepoMock.Object, _currencyRateRepoMock.Object, _exchangeInfoRepoMock.Object, _companyExchangeFundsRepoMock.Object, _bookKeepingRepoMock.Object, _userServiceMock.Object);
    }

    [Test]
    public async Task DoExchange_Fails_If_SourceOrTargetCurrency_IsNull()
    {
        int userId = 1;
        int sourceCurrencyId = 100;
        int targetCurrencyId = 200;
        decimal amount = 500m;

        // i set up the repository so that it'll return an empty list of currencyentities, meaning no currencies were found in the db with given ids
        _currencyRepoMock.Setup(repo =>
                repo.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<CurrencyEntity>()); 

        ExchangeInfoEntity? capturedExchangeInfo = null;

        // i set up the exchaneinforepository to create an exchangeinfoentity 
        // what's the purpose of callback?
        _exchangeInfoRepoMock.Setup(repo =>
                repo.Create(It.IsAny<ExchangeInfoEntity>()))
            .Callback<ExchangeInfoEntity>(e => capturedExchangeInfo = e);
        
        // this part with the verifiable at the end means that i'll want to check later that this update call was indeed called?
        _exchangeInfoRepoMock.Setup(repo =>
                repo.Update(It.IsAny<ExchangeInfoEntity>()))
            .Verifiable();
        
        // act 
        await _exchangeService.DoExchange(userId, sourceCurrencyId, targetCurrencyId, amount);
        
        // what we marked verifiable before, now we want to see what the exchangeinforepository actually produced - and we anticipate it to produce an exchangeinfoentity with a failed status, and a non-null value for the failedat property, also that it was called once
        _exchangeInfoRepoMock.Verify(repo =>
            repo.Update(It.Is<ExchangeInfoEntity>(e =>
                    e.Status == ExchangeStatus.Failed &&
                    e.FailedAt != null                     
            )), Times.Once);
        
        Assert.That(capturedExchangeInfo, Is.Not.Null);
        Assert.That(capturedExchangeInfo.Status, Is.EqualTo(ExchangeStatus.Failed));
    }
    
    [Test]
    public async Task DoExchange_Fails_WhenAmountIsZeroOrNegative()
    {
        int userId = 1;
        int sourceCurrencyId = 100;
        int targetCurrencyId = 200;
        decimal amount = 0;

        // we set up the currencyrepository so that it returns 2 valid currency entities
        _currencyRepoMock.Setup(repo => repo.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<CurrencyEntity>
            {
                new() { Id = sourceCurrencyId, Name = "USD", ShortName = "USD" },
                new() { Id = targetCurrencyId, Name = "EUR", ShortName = "EUR" }
            });
        
        // again, we set up the exchangeinforepository so that it's create method produces an exchangeinfoentity  
        _exchangeInfoRepoMock.Setup(repo => repo.Create(It.IsAny<ExchangeInfoEntity>()));
        
        // we'll want to verify later, that we will update our exchangeinfo entity's status and failedat
        _exchangeInfoRepoMock.Setup(repo => repo.Update(It.Is<ExchangeInfoEntity>(e =>
            e.Status == ExchangeStatus.Failed &&
            e.FailedAt != null
        ))).Verifiable();

        await _exchangeService.DoExchange(userId, sourceCurrencyId, targetCurrencyId, amount);

        // we verify here
        _exchangeInfoRepoMock.Verify();
    }

    [Test]
    public async Task DoExchange_Fails_WhenUserHasNoSourceCurrencyFund()
    {
        int userId = 1;
        int sourceCurrencyId = 100;
        int targetCurrencyId = 200;
        decimal amount = 100;

        _currencyRepoMock.Setup(repository => repository.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<CurrencyEntity>
            {
                new() { Id = sourceCurrencyId, Name = "Dollar", ShortName = "USD" },
                new() { Id = targetCurrencyId, Name = "Euro", ShortName = "EUR" }
            });

        // we set up the userservice so that the model it returns, has no funds to it, meaning user has no money to change
        _userServiceMock.Setup(svc => svc.GetById(userId))
            .ReturnsAsync(new UserModel(
                userId,
                "Test",
                "User",
                new List<UserFundModel>()
            ));

        _exchangeInfoRepoMock.Setup(repository => repository.Create(It.IsAny<ExchangeInfoEntity>()));
        
        _exchangeInfoRepoMock.Setup(repo => repo.Update(It.Is<ExchangeInfoEntity>(e =>
            e.Status == ExchangeStatus.Failed
        ))).Verifiable();

        await _exchangeService.DoExchange(userId, sourceCurrencyId, targetCurrencyId, amount);

        _exchangeInfoRepoMock.Verify();
    }
    
    [Test]
    public async Task DoExchange_Fails_WhenUserHasInsufficientFunds()
    {
        int userId = 1;
        int sourceCurrencyId = 100;
        int targetCurrencyId = 200;
        decimal amount = 200;

        _currencyRepoMock.Setup(repo => repo.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<CurrencyEntity>
            {
                new() { Id = sourceCurrencyId, Name = "USD", ShortName = "USD" },
                new() { Id = targetCurrencyId, Name = "EUR", ShortName = "EUR" }
            });

        var currencyModel = new CurrencyModel(sourceCurrencyId, "USD", "USD");

        var user = new UserModel(
            Id: userId,
            FirstName: "John",
            LastName: "Doe",
            Funds: new List<UserFundModel>
            {
                new UserFundModel(
                    Id: 1,
                    UserId: userId,
                    CurrencyModel: currencyModel,
                    Pending: 0,
                    // focus is on this line below, because user only has a hundred points of money, but he intends to change 200, so exchange should fail here
                    Disposable: 100
                )
            }
        );

        _userServiceMock.Setup(svc => svc.GetById(userId)).ReturnsAsync(user);

        _exchangeInfoRepoMock.Setup(repo => repo.Create(It.IsAny<ExchangeInfoEntity>()));
        _exchangeInfoRepoMock.Setup(repo => repo.Update(It.Is<ExchangeInfoEntity>(e =>
            e.Status == ExchangeStatus.Failed
        ))).Verifiable();

        await _exchangeService.DoExchange(userId, sourceCurrencyId, targetCurrencyId, amount);

        _exchangeInfoRepoMock.Verify();
    }
}