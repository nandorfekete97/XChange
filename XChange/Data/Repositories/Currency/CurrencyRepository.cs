using Microsoft.EntityFrameworkCore;
using XChange.Data.context;
using XChange.Data.Entities;

namespace XChange.Data.Repositories.Currency;

public class CurrencyRepository : ICurrencyRepository
{
    private XChangeContext _dbContext;

    public CurrencyRepository(XChangeContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<CurrencyEntity> GetById(int id)
    {
        return await _dbContext.Currencies.FirstOrDefaultAsync(currency => currency
            .Id == id);
    }

    public async Task<List<CurrencyEntity>> GetByIds(List<int> ids)
    {
        return await _dbContext.Currencies.Where(currency => ids.Contains(currency.Id)).ToListAsync();
    }

    public async Task<List<CurrencyEntity>> GetAll()
    {
        return await _dbContext.Currencies.ToListAsync();
    }

    public async Task Create(CurrencyEntity currency)
    {
        _dbContext.Currencies.AddAsync(currency);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteById(int currencyId)
    {
        var currencyToDelete = await GetById(currencyId);

        if (currencyToDelete is not null)
        {
            _dbContext.Currencies.Remove(currencyToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}