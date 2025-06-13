using XChange.Data.Entities;

namespace XChange.Services;

public interface IExchangeService
{
    Task<ExchangeInfoEntity> GetById(int exchangeInfoId);
    Task<List<ExchangeInfoEntity>> GetByIds(List<int> exchangeInfoIds);
    Task<List<ExchangeInfoEntity>> GetByUserId(int userId);
    Task DeleteById(int exchangeInfoId);
    Task DoExchange(int userId, int sourceCurrencyId, int targetCurrencyId, decimal amount);
}