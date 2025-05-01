namespace XChange.Services;

public interface IExchangeService
{
    Task DoExchange(int userId, int sourceCurrencyId, int targetCurrencyId, decimal amount);
}