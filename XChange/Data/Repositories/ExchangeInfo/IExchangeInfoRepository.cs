using XChange.Data.Entities;

namespace XChange.Data.Repositories.ExchangeInfo;

public interface IExchangeInfoRepository
{
    Task<ExchangeInfoEntity> GetById(int id);
    Task<List<ExchangeInfoEntity>> GetByIds(List<int> ids);
    Task<List<ExchangeInfoEntity>> GetByUserId(int userId);
    Task Create(ExchangeInfoEntity exchangeInfoEntity);
    Task Update(ExchangeInfoEntity exchangeInfoEntity);
    Task<bool> DeleteById(int id);
}