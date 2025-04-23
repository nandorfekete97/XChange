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

        _currencyRepoMock.Setup(repo =>
                repo.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<CurrencyEntity>()); 

        ExchangeInfoEntity? capturedExchangeInfo = null;

        _exchangeInfoRepoMock.Setup(repo =>
                repo.Create(It.IsAny<ExchangeInfoEntity>()))
            .Callback<ExchangeInfoEntity>(e => capturedExchangeInfo = e);
        
        _exchangeInfoRepoMock.Setup(repo =>
                repo.Update(It.IsAny<ExchangeInfoEntity>()))
            .Verifiable();
        
        // act 
        await _exchangeService.DoExchange(userId, sourceCurrencyId, targetCurrencyId, amount);
        
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

        _currencyRepoMock.Setup(repo => repo.GetByIds(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<CurrencyEntity>
            {
                new() { Id = sourceCurrencyId, Name = "USD", ShortName = "USD" },
                new() { Id = targetCurrencyId, Name = "EUR", ShortName = "EUR" }
            });
        
        _exchangeInfoRepoMock.Setup(repo => repo.Create(It.IsAny<ExchangeInfoEntity>()));
        
        _exchangeInfoRepoMock.Setup(repo => repo.Update(It.Is<ExchangeInfoEntity>(e =>
            e.Status == ExchangeStatus.Failed &&
            e.FailedAt != null
        ))).Verifiable();

        await _exchangeService.DoExchange(userId, sourceCurrencyId, targetCurrencyId, amount);

        _exchangeInfoRepoMock.Verify();
    }

    [Test]
    public async Task DoExchange_Fails_WhenUserHasNoSrouceCurrencyFund()
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

        _userServiceMock.Setup(svc => svc.GetUserById(userId))
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
                    CurrencyModel: currencyModel,
                    Pending: 0,
                    Disposable: 100
                )
            }
        );

        _userServiceMock.Setup(svc => svc.GetUserById(userId)).ReturnsAsync(user);

        _exchangeInfoRepoMock.Setup(repo => repo.Create(It.IsAny<ExchangeInfoEntity>()));
        _exchangeInfoRepoMock.Setup(repo => repo.Update(It.Is<ExchangeInfoEntity>(e =>
            e.Status == ExchangeStatus.Failed
        ))).Verifiable();

        await _exchangeService.DoExchange(userId, sourceCurrencyId, targetCurrencyId, amount);

        _exchangeInfoRepoMock.Verify();
    }
    
    [Test]
public async Task DoExchange_Succeeds_WhenAllConditionsAreMet()
{
    int userId = 1;
    int sourceCurrencyId = 100;
    int targetCurrencyId = 200;
    decimal amount = 100;

    _currencyRepoMock.Setup(repo => repo.GetByIds(It.IsAny<List<int>>()))
        .ReturnsAsync(new List<CurrencyEntity>
        {
            new() { Id = sourceCurrencyId, Name = "USD", ShortName = "USD" },
            new() { Id = targetCurrencyId, Name = "EUR", ShortName = "EUR" }
        });

    var sourceCurrencyModel = new CurrencyModel(sourceCurrencyId, "USD", "USD");
    var targetCurrencyModel = new CurrencyModel(targetCurrencyId, "EUR", "EUR");

    var user = new UserModel(
        Id: userId,
        FirstName: "Jane",
        LastName: "Smith",
        Funds: new List<UserFundModel>
        {
            new UserFundModel(1, sourceCurrencyModel, Pending: 0, Disposable: 200),
            new UserFundModel(2, targetCurrencyModel, Pending: 0, Disposable: 0)
        }
    );

    _userServiceMock.Setup(svc => svc.GetUserById(userId)).ReturnsAsync(user);

    _currencyRateRepoMock.Setup(repo => repo.GetLastCurrencyRateByCurrencyIds(It.IsAny<List<int>>()))
        .ReturnsAsync(new Dictionary<int, CurrencyRateEntity>
        {
            { targetCurrencyId, new CurrencyRateEntity { Id = 5, Rate = 2.0m, CurrencyId = targetCurrencyId } }
        });

    _exchangeInfoRepoMock.Setup(repo => repo.Create(It.IsAny<ExchangeInfoEntity>()));

    var updatedExchangeEntities = new List<ExchangeInfoEntity>();

    _exchangeInfoRepoMock.Setup(repo => repo.Update(It.IsAny<ExchangeInfoEntity>()))
        .Callback<ExchangeInfoEntity>(entity =>
        {
            // Capture a snapshot of the values we care about
            updatedExchangeEntities.Add(new ExchangeInfoEntity
            {
                Id = entity.Id,
                Status = entity.Status,
                // Add other properties here if you want to inspect them later
            });
        });

    _userFundsRepoMock.Setup(repo => repo.Update(It.IsAny<UserFundEntity>()));

    // Act
    await _exchangeService.DoExchange(userId, sourceCurrencyId, targetCurrencyId, amount);

    // Assert
    Assert.That(updatedExchangeEntities.Count(e => e.Status == ExchangeStatus.Successful), Is.EqualTo(1));

    _userFundsRepoMock.Verify(repo => repo.Update(It.IsAny<UserFundEntity>()), Times.Exactly(3));
}

}