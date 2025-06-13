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
    public async Task<ExchangeInfoEntity> GetById(int exchangeInfoId)
    {
        if (exchangeInfoId <= 0)
        {
            throw new ArgumentException("ID must be positive integer.");
        }

        return await exchangeInfoRepository.GetById(exchangeInfoId);
    }

    public async Task<List<ExchangeInfoEntity>> GetByIds(List<int> exchangeInfoIds)
    {
        if (exchangeInfoIds.Any(id => id <= 0))
        {
            throw new ArgumentException("All IDs must be positive integers.");
        }
        
        return await exchangeInfoRepository.GetByIds(exchangeInfoIds);
    }

    public async Task<List<ExchangeInfoEntity>> GetByUserId(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("ID must be positive integer.");
        }

        return await exchangeInfoRepository.GetByUserId(userId);
    }

    public async Task DeleteById(int exchangeInfoId)
    {
        ExchangeInfoEntity exchangeInfoToDelete = await exchangeInfoRepository.GetById(exchangeInfoId);

        if (exchangeInfoToDelete == null)
        {
            throw new ArgumentException("ExchangeInfo ID not found, could not be deleted.");
        }

        await exchangeInfoRepository.DeleteById(exchangeInfoId);
    }

    public async Task DoExchange(int userId, int sourceCurrencyId, int targetCurrencyId, decimal amount)
    {
        // letrehozok egy exchangeinfoentityt
        // exchangeinfo egy penzvaltas dokumentacioja, alapbol csak ugy johet letre hogyha tudjuk a userid-t, sourcecurrency es targetcurrency id-t, hogy mennyi penzt ohajtunk valtani
        // az exchange statusnak mindig a megadott 4 enumbol kell az egyiknek lennie, nem lehet null
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

        // stage1: feltoltjuk a db-be az exchangeinfoentityt
        await exchangeInfoRepository.Create(exchangeInfoEntity);
        
        // a currency id-k alapjan megszerezzuk a currency entityket
        List<CurrencyEntity> currencyEntities = await currencyRepository.GetByIds([sourceCurrencyId, targetCurrencyId]);
        
        // id alapjan azonositjuk oket a listabol amit kaptunk fent
        CurrencyEntity? sourceCurrency = currencyEntities.FirstOrDefault(entity => entity.Id == sourceCurrencyId);
        CurrencyEntity? targetCurrency = currencyEntities.FirstOrDefault(entity => entity.Id == targetCurrencyId);

        // hogyha valamelyik currency nincs, failelnie kell az exchangenek
        if (sourceCurrency == null || targetCurrency == null)
        {
            HandleTransactionFailForEntity(exchangeInfoEntity);
            return;
        }

        // ha az amount kisebb mint 0, failelnie kell az exchangenek
        if (amount <= 0)
        {
            HandleTransactionFailForEntity(exchangeInfoEntity);
            return;
        }
        
        // megszerezzuk a usermodelt id alapjan
        UserModel userModel = await _userService.GetById(userId);

        // kideritjuk hogy a usernek mennyi penze van abbol a penznembol, amibol valt
        UserFundModel? sourceCurrencyFund = userModel.Funds.FirstOrDefault(fund => fund.CurrencyModel.Id == sourceCurrencyId);

        // ha nincs az adott penznembol penze, failelnie kell az exchangenek
        if (sourceCurrencyFund == null)
        {
            HandleTransactionFailForEntity(exchangeInfoEntity);
            return;
        }

        // ha az adott penznembol nincs annyi penze amennyit valtani akar, failelnie kell az exchangenek
        if (sourceCurrencyFund.Disposable < amount)
        {
            HandleTransactionFailForEntity(exchangeInfoEntity);
            return;
        }

        // ha minden filteren atmentunk, megvaltoztatjuk az exchangestatust
        // updateljuk az exchangeinfot
        exchangeInfoEntity.Status = ExchangeStatus.Accepted;
        exchangeInfoRepository.Update(exchangeInfoEntity);

        // megszerezzuk a userfundot a db-bol, es frissitjuk a propertyjeit
        // a userfundentity tartalmaz userid-t, szoval minden user minden fundjanak kulon entityje van
        UserFundEntity sourceCurrencyUserFundEntity = await userFundsRepository.GetById(sourceCurrencyFund.Id);
        
        if (sourceCurrencyUserFundEntity == null)
        {
            throw new Exception($"User {userId} does not have funds in source currency {sourceCurrencyId}");
        }
        
        sourceCurrencyUserFundEntity.Disposable -= amount;
        sourceCurrencyUserFundEntity.Pending += amount;
        await userFundsRepository.Update(sourceCurrencyUserFundEntity);

        // megszerezzuk a targetcurrency utolso currency rate-jet, avagy
        // kideritjuk hogy mennyivel valtunk a penzre, amire valtunk
        Dictionary<int, CurrencyRateEntity> targetCurrencyIdWithLatestCurrencyRate = 
            await currencyRateRepository.GetLastCurrencyRateByCurrencyIds([targetCurrencyId]);
        decimal latestCurrencyRate = targetCurrencyIdWithLatestCurrencyRate[targetCurrencyId].Rate;
        
        // az exchangeinfo entity tartalmazza az currencyrate id-t
        // ez azert kell hogy tudjuk hogy a penz mi alapjan lett valtva, mennyi volt a rate a valtaskor
        exchangeInfoEntity.CurrencyRateId = targetCurrencyIdWithLatestCurrencyRate[targetCurrencyId].Id;
        await exchangeInfoRepository.Update(exchangeInfoEntity);
        
        // megszerezzuk a targetcurrencyfundot, azaz azt a fundot, amire valtunk
        // hogyha valtok forintra, akkor a forintomnak novekednie kell => frissitjuk az ertekeket
        UserFundEntity targetCurrencyUserFundEntity = await GetUserFundEntityForTargetCurrencyId(userModel, targetCurrencyId);
        targetCurrencyUserFundEntity.Disposable += amount * latestCurrencyRate;
        await userFundsRepository.Update(targetCurrencyUserFundEntity);
        
        sourceCurrencyUserFundEntity.Pending -= amount;
        await userFundsRepository.Update(sourceCurrencyUserFundEntity);

        exchangeInfoEntity.Status = ExchangeStatus.Successful;
        exchangeInfoEntity.FinalizedAt = DateTime.UtcNow;
        await exchangeInfoRepository.Update(exchangeInfoEntity);
    }

    private void HandleTransactionFailForEntity(ExchangeInfoEntity exchangeInfoEntity)
    {
        exchangeInfoEntity.FailedAt = DateTime.Now;
        exchangeInfoEntity.Status = ExchangeStatus.Failed;
        exchangeInfoRepository.Update(exchangeInfoEntity);
    }

    private async Task<UserFundEntity> GetUserFundEntityForTargetCurrencyId(UserModel userModel, int currencyId)
    {
        UserFundModel? userFundModel = userModel.Funds.FirstOrDefault(model => model.CurrencyModel.Id == currencyId);

        if (userFundModel != null)
        {
            return await userFundsRepository.GetById(userFundModel.Id);
        }
        
        UserFundEntity newTargetCurrencyFund = new UserFundEntity
        {
            CurrencyId = currencyId,
            UserId = userModel.Id,
            Disposable = 0,
            Pending = 0
        };

        return await userFundsRepository.Create(newTargetCurrencyFund);
    }
}