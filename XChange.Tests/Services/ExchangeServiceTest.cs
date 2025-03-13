using System.Data;
using Moq;
using NUnit.Framework;
using XChange.Data.Repositories.BookKeeping;
using XChange.Data.Repositories.CompanyExchangeFunds;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.CurrencyRate;
using XChange.Data.Repositories.User;
using XChange.Data.Repositories.UserFunds;
using XChange.Services;

namespace XChange.Tests.Services;

[TestFixture]
[TestOf(typeof(ExchangeService))]
public class ExchangeServiceTest
{
    private Mock<IUserRepository> _userRepoMock;
    private Mock<IUserFundsRepository> _userFundsRepoMock;
    private Mock<ICurrencyRepository> _currencyRepoMock;
    private Mock<ICurrencyRateRepository> _currencyRateRepoMock;
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
    _companyExchangeFundsRepoMock = new Mock<ICompanyExchangeFundsRepository>();
    _bookKeepingRepoMock = new Mock<IBookKeepingRepository>();
    _userServiceMock = new Mock<IUserService>();
    _exchangeService = new ExchangeService(_userRepoMock.Object, _userFundsRepoMock.Object, _currencyRepoMock.Object, _currencyRateRepoMock.Object, _companyExchangeFundsRepoMock.Object, _bookKeepingRepoMock.Object, _userServiceMock.Object);
    }

    [Test]
    public void Test_DoExchange_Fails_If_SourceOrTargetCurrency_IsNull()
    {
        
    }
}