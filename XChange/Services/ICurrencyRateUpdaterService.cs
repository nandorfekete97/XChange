using XChange.Data.Repositories.CurrencyRate;

namespace XChange.Services;

public interface ICurrencyRateUpdaterService
{
    Task UpdateCurrencyRates(CancellationToken cancellationToken);
}