using Microsoft.EntityFrameworkCore;
using XChange.Data.context;
using XChange.Data.Entities;

namespace XChange.Data.Repositories.ExchangeInfo;

public class ExchangeInfoRepository : IExchangeInfoRepository
{
    private XChangeContext _dbContext;

    public ExchangeInfoRepository(XChangeContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ExchangeInfoEntity> GetById(int id)
    {
        return await _dbContext.ExchangeInfos.FirstOrDefaultAsync(entity => entity.Id == id);
    }
    
    public async Task<List<ExchangeInfoEntity>> GetByIds(List<int> ids)
     {
         return await _dbContext.ExchangeInfos.Where(entity => ids.Contains(entity.Id)).ToListAsync();
     }

    public async Task Create(ExchangeInfoEntity exchangeInfoEntity)
    {
        _dbContext.ExchangeInfos.AddAsync(exchangeInfoEntity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(ExchangeInfoEntity exchangeInfoEntity)
    {
        _dbContext.Update(exchangeInfoEntity);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<bool> DeleteById(int id)
    {
        var exchangeInfoToDelete = await _dbContext.ExchangeInfos.FirstOrDefaultAsync(entity => entity.Id == id);

        if (exchangeInfoToDelete is not null)
        {
            _dbContext.Remove(exchangeInfoToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        
        return false;    
    }
}