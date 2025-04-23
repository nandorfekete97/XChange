using XChange.Data.Entities;

namespace XChange.Data.Repositories.Currency;

public interface ICurrencyRepository
{
    Task<CurrencyEntity> GetById(int id);
    Task<List<CurrencyEntity>> GetByIds(List<int> ids);
    Task<List<CurrencyEntity>> GetAll();
    Task Create(CurrencyEntity currency);
    Task DeleteById(int currencyId);
}