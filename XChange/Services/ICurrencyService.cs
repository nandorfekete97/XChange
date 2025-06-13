using XChange.Data.Entities;
using XChange.Models;

namespace XChange.Services
{
    public interface ICurrencyService
    {
        Task<List<CurrencyRateModel>> GetLastCurrencyRatesByCurrencyIds(List<int> currencyIds);
        Task<CurrencyModel> GetById(int id);
        Task<List<CurrencyModel>> GetByIds(List<int> ids);
        Task<List<CurrencyModel>> GetAll();
        Task Create(CurrencyModel currency);
        Task DeleteById(int currencyId);
    }
}