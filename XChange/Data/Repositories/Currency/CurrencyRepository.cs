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

    public async Task AddAsync(CurrencyEntity currency)
    {
        _dbContext.Currencies.AddAsync(currency);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(CurrencyEntity currency)
    {
        _dbContext.Currencies.Remove(currency);
        await _dbContext.SaveChangesAsync();
    }
}