using XChange.Data.Entities;

namespace XChange.Data.Repositories.Currency;

public interface ICurrencyRepository
{
    public Task<CurrencyEntity> GetById(int id);
    public Task<List<CurrencyEntity>> GetByIds(List<int> ids);
    public Task AddAsync(CurrencyEntity currency);
    public Task DeleteAsync(CurrencyEntity currency);
}