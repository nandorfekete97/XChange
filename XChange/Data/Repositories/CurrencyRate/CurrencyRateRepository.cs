using Microsoft.EntityFrameworkCore;
using XChange.Data.context;
using XChange.Data.Entities;

namespace XChange.Data.Repositories.CurrencyRate;

public class CurrencyRateRepository : ICurrencyRateRepository
{
    private XChangeContext _dbContext;

    public CurrencyRateRepository(XChangeContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CurrencyRateEntity> GetById(int id)
    {
        return await _dbContext.CurrencyRates.FirstOrDefaultAsync(currencyRate => currencyRate.Id == id);
    }

    public async Task<List<CurrencyRateEntity>> GetByCurrencyId(int currencyId)
    {
        return await _dbContext.CurrencyRates.Where(entity => entity.CurrencyId == currencyId).ToListAsync();
    }
    
    public async Task<Dictionary<int, CurrencyRateEntity>> GetLastCurrencyRateByCurrencyIds(List<int> currencyIds)
    {
        List<CurrencyRateEntity> currencyRates = await _dbContext.CurrencyRates
            .Where(currencyRate => currencyIds.Contains(currencyRate.CurrencyId))
            .ToListAsync();

        Dictionary<int, CurrencyRateEntity> currencyToCurrencyRate = new Dictionary<int, CurrencyRateEntity>();

        foreach (var currencyId in currencyIds)
        {
            CurrencyRateEntity mostRecent = currencyRates
                .Where(currencyRate => currencyRate.CurrencyId == currencyId)
                .MaxBy(currencyRate => currencyRate.Timestamp)!;
            
            currencyToCurrencyRate.Add(currencyId, mostRecent);
        }

        return currencyToCurrencyRate;
    }

    public async Task Create(CurrencyRateEntity currencyRate)
    {
        _dbContext.CurrencyRates.AddAsync(currencyRate);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteById(int currencyRateId)
    {
        var currencyRateToDelete =
            await _dbContext.CurrencyRates.FirstOrDefaultAsync(currencyRate => currencyRate.Id == currencyRateId);

        if (currencyRateToDelete is not null)
        {
            _dbContext.Remove(currencyRateToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
}