using XChange.Data.Entities;
using XChange.Data.Repositories.BookKeeping;
using XChange.Data.Repositories.CompanyExchangeFunds;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.CurrencyRate;
using XChange.Data.Repositories.ExchangeInfo;
using XChange.Data.Repositories.User;
using XChange.Data.Repositories.UserFunds;
using XChange.Models;

namespace XChange.Services;

public class ExchangeService(
    IUserRepository userRepository,
    IUserFundsRepository userFundsRepository,
    ICurrencyRepository currencyRepository,
    ICurrencyRateRepository currencyRateRepository,
    IExchangeInfoRepository exchangeInfoRepository,
    ICompanyExchangeFundsRepository companyExchangeFundsRepository,
    IBookKeepingRepository bookKeepingRepository,
    IUserService _userService) : IExchangeService
{
    
    public async Task DoExchange(int userId, int sourceCurrencyId, int targetCurrencyId, decimal amount)
    {
        // user kezdemenyez exchange requestet - a requestben benne van a currency amire valt (pl. forint); tudni kell mennyi penzt akar valtani
        // updatelni kell a userfunds propertyket (pending, disposable); 
        // kell krealni egy exchangeinfo entity-t (user, penzosszeg)...
        // exchange statust updatelni kell => 
        // ha a user adatok megfelelnek (van eleg penze pl.), akkor verification, amikor vegbement => successful 
        // bookkeeping entry ha sikeres volt a valtas 

        ExchangeInfoEntity exchangeInfoEntity = new ExchangeInfoEntity
        {
            FinalizedAt = null,
            StartedAt = DateTime.Now,
            FailedAt = null,
            UserId = userId,
            SourceCurrencyId = sourceCurrencyId,
            TargetCurrencyId = targetCurrencyId,
            CurrencyRateId = null,
            SourceCurrencyAmount = amount,
            Status = ExchangeStatus.Verification,
            Error = null
        };

        exchangeInfoRepository.Create(exchangeInfoEntity);
        
        List<CurrencyEntity> currencyEntities = await currencyRepository.GetByIds([sourceCurrencyId, targetCurrencyId]);

        CurrencyEntity? sourceCurrency = currencyEntities.FirstOrDefault(entity => entity.Id == sourceCurrencyId);
        CurrencyEntity? targetCurrency = currencyEntities.FirstOrDefault(entity => entity.Id == targetCurrencyId);

        if (sourceCurrency == null || targetCurrency == null)
        {
            HandleTransactionFailForEntity(exchangeInfoEntity);
            //exchangeInfoEntity.Error = 
            // throw new ArgumentException("One or both currencies are non existent.");
            return;
        }

        if (amount <= 0)
        {
            HandleTransactionFailForEntity(exchangeInfoEntity);

            //exchangeInfoEntity.Error = 
            //throw new ArgumentException("Exchange amount cannot be zero or negative.");
            return;
        }
        
        UserModel userModel = await _userService.GetById(userId);

        UserFundModel? sourceCurrencyFund = userModel.Funds.FirstOrDefault(fund => fund.CurrencyModel.Id == sourceCurrencyId);

        if (sourceCurrencyFund == null)
        {
            HandleTransactionFailForEntity(exchangeInfoEntity);

            //throw new ArgumentException("User doesn't own specified currency.");
            return;
        }

        if (sourceCurrencyFund.Disposable < amount)
        {
            HandleTransactionFailForEntity(exchangeInfoEntity);

            //throw new ArgumentException("Insufficient funds.");
            return;
        }

        exchangeInfoEntity.Status = ExchangeStatus.Accepted;
        exchangeInfoRepository.Update(exchangeInfoEntity);

        UserFundEntity sourceCurrencyUserFundEntity = new UserFundEntity(
            userModel.Id,
            sourceCurrencyId,
            sourceCurrencyFund.Pending + amount,
            sourceCurrencyFund.Disposable - amount
        )
        {
            Id = sourceCurrencyFund.Id 
        };
        
        await userFundsRepository.Update(sourceCurrencyUserFundEntity);

        Dictionary<int, CurrencyRateEntity> targetCurrencyIdWithLatestCurrencyRate =
            await currencyRateRepository.GetLastCurrencyRateByCurrencyIds([targetCurrencyId]);
        decimal latestCurrencyRate = targetCurrencyIdWithLatestCurrencyRate[targetCurrencyId].Rate;
        
        UserFundModel targetCurrencyFund = userModel.Funds.First(fund => fund.CurrencyModel.Id == targetCurrencyId);

        UserFundEntity targetCurrencyUserFundEntity = new UserFundEntity(
            userModel.Id,
            targetCurrencyId,
            targetCurrencyFund.Pending,
            targetCurrencyFund.Disposable + (amount * latestCurrencyRate)
        )
        {
            Id = targetCurrencyFund.Id
        };

        await userFundsRepository.Update(targetCurrencyUserFundEntity);

        sourceCurrencyUserFundEntity.Pending -= amount;
        await userFundsRepository.Update(sourceCurrencyUserFundEntity);

        exchangeInfoEntity.Status = ExchangeStatus.Successful;
        await exchangeInfoRepository.Update(exchangeInfoEntity);
    }

    private void HandleTransactionFailForEntity(ExchangeInfoEntity exchangeInfoEntity)
    {
        exchangeInfoEntity.FailedAt = DateTime.Now;
        exchangeInfoEntity.Status = ExchangeStatus.Failed;
        exchangeInfoRepository.Update(exchangeInfoEntity);
    }
}