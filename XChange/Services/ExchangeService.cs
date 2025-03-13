using XChange.Data.Entities;
using XChange.Data.Repositories.BookKeeping;
using XChange.Data.Repositories.CompanyExchangeFunds;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.CurrencyRate;
using XChange.Data.Repositories.User;
using XChange.Data.Repositories.UserFunds;
using XChange.Models;

namespace XChange.Services;

public class ExchangeService(
    IUserRepository userRepository,
    IUserFundsRepository userFundsRepository,
    ICurrencyRepository currencyRepository,
    ICurrencyRateRepository currencyRateRepository,
    ICompanyExchangeFundsRepository companyExchangeFundsRepository,
    IBookKeepingRepository bookKeepingRepository,
    IUserService _userService)
{
    
    public async Task DoExchange(int userId, int sourceCurrencyId, int targetCurrencyId, decimal amount)
    {
        // user kezdemenyez exchange requestet - a requestben benne van a currency amire valt (pl. forint); tudni kell mennyi penzt akar valtani
        // updatelni kell a userfunds propertyket (pending, disposable); 
        // kell krealni egy exchangeinfo entity-t (user, penzosszeg)...
        // exchange statust updatelni kell => 
        // ha a user adatok megfelelnek (van eleg penze pl.), akkor verification, amikor vegbement => successful 
        // bookkeeping entry ha sikeres volt a valtas 

        List<CurrencyEntity> currencyEntities = await currencyRepository.GetByIds([sourceCurrencyId, targetCurrencyId]);

        CurrencyEntity? sourceCurrency = currencyEntities.FirstOrDefault(entity => entity.Id == sourceCurrencyId);
        CurrencyEntity? targetCurrency = currencyEntities.FirstOrDefault(entity => entity.Id == targetCurrencyId);

        if (sourceCurrency == null || targetCurrency == null)
        {
            throw new ArgumentException("One or both currencies are non existent.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Exchange amount cannot be zero or negative.");
        }
        
        UserModel userModel = await _userService.GetUserById(userId);

        UserFundModel? sourceCurrencyFund = userModel.Funds.FirstOrDefault(fund => fund.CurrencyModel.Id == sourceCurrencyId);

        if (sourceCurrencyFund == null)
        {
            throw new ArgumentException("User doesn't own specified currency.");
        }

        if (sourceCurrencyFund.Disposable < amount)
        {
            throw new ArgumentException("Insufficient funds.");
        }

        UserFundEntity sourceCurrencyUserFundEntity = new UserFundEntity(
            sourceCurrencyFund.Id,
            userModel.Id, 
            sourceCurrencyId,
            sourceCurrencyFund.Pending + amount,
            sourceCurrencyFund.Disposable - amount);

        await userFundsRepository.UpdateUserFunds(sourceCurrencyUserFundEntity);

        Dictionary<int, CurrencyRateEntity> targetCurrencyIdWithLatestCurrencyRate =
            await currencyRateRepository.GetLastCurrencyRateByCurrencyIds([targetCurrencyId]);
        int latestCurrencyRateId = targetCurrencyIdWithLatestCurrencyRate[targetCurrencyId].Id;

        ExchangeInfoEntity exchangeInfoEntity = new ExchangeInfoEntity(
            null,
            null,
            DateTime.Now,
            null,
            userModel.Id,
            sourceCurrencyId,
            targetCurrencyId,
            latestCurrencyRateId,
            amount,
            ExchangeStatus.Accepted,
            null
        );
        
        // TODO: status handling refactor 

    }
    
}