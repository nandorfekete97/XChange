using XChange.Data.Entities;

namespace XChange.Data.Repositories.ExchangeInfo;

public interface IExchangeInfoRepository
{
    Task<ExchangeInfoEntity> GetById(int id);
    //Task<List<ExchangeInfoEntity>> GetByIds(List<int> ids);
    Task AddAsync(ExchangeInfoEntity exchangeInfoEntity);
    Task UpdateAsync(ExchangeInfoEntity exchangeInfoEntity);
    Task<bool> DeleteAsync(int id);
}