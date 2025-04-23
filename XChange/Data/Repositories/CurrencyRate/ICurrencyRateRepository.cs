using XChange.Data.Entities;

namespace XChange.Data.Repositories.CurrencyRate;

public class InClassName
{
    public InClassName(CurrencyRateEntity currencyRate)
    {
        CurrencyRate = currencyRate;
    }

    public CurrencyRateEntity CurrencyRate { get; private set; }
}

public interface ICurrencyRateRepository
{
    public Task<CurrencyRateEntity> GetById(int id);
    public Task<List<CurrencyRateEntity>> GetByCurrencyId(int currencyId);

    public Task<Dictionary<int, CurrencyRateEntity>> GetLastCurrencyRateByCurrencyIds(List<int> currencyIds);
    public Task Create(CurrencyRateEntity currencyRate);
    //public Task<bool> DeleteAsync(InClassName inClassName);
    public Task<bool> DeleteById(int currencyRateId);
}