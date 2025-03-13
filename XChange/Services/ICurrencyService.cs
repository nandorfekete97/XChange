using XChange.Data.Entities;
using XChange.Models;

namespace XChange.Services
{
    public interface ICurrencyService
    {
        Task<List<CurrencyRateModel>> GetLastCurrencyRatesByCurrencyIds(List<int> currencyIds);
        CurrencyRateModel ConvertCurrencyRateEntityToModel(CurrencyRateEntity currencyRateEntity, CurrencyModel currencyModel);
        CurrencyModel ConvertCurrencyEntityToModel(CurrencyEntity currencyEntity);
    }
}