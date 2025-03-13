using System.Collections.Immutable;
using XChange.Data.Entities;
using XChange.Data.Repositories.Currency;
using XChange.Data.Repositories.CurrencyRate;
using XChange.Models;

namespace XChange.Services;

public class CurrencyService(
    ICurrencyRepository currencyRepository, 
    ICurrencyRateRepository currencyRateRepository)
{
    
    public async Task<List<CurrencyRateModel>> GetLastCurrencyRatesByCurrencyIds(List<int> currencyIds)
    {
        List<CurrencyRateModel> currencyRateModels = new List<CurrencyRateModel>();

        Dictionary<int, CurrencyRateEntity> currencyIdToMostRecentCurrencyRates = await currencyRateRepository.GetLastCurrencyRateByCurrencyIds(currencyIds);
        
        List<CurrencyEntity> currencyEntities = await currencyRepository.GetByIds(currencyIds);
        
        foreach (var currencyEntity in currencyEntities)
        {
            CurrencyRateEntity mostRecentCurrencyRateEntity = currencyIdToMostRecentCurrencyRates[currencyEntity.Id];

            CurrencyModel currencyModel = ConvertCurrencyEntityToModel(currencyEntity);

            CurrencyRateModel currencyRateModel = ConvertCurrencyRateEntityToModel(mostRecentCurrencyRateEntity, currencyModel);

            currencyRateModels.Add(currencyRateModel);
        }

        return currencyRateModels;
    }

    public CurrencyRateModel ConvertCurrencyRateEntityToModel(CurrencyRateEntity currencyRateEntity, CurrencyModel currencyModel)
    {
        return new CurrencyRateModel(currencyModel, currencyRateEntity.Rate);
    }
    
    public CurrencyModel ConvertCurrencyEntityToModel(CurrencyEntity currencyEntity)
    {
        return new CurrencyModel(currencyEntity.Id, currencyEntity.Name, currencyEntity.ShortName);
    }
}