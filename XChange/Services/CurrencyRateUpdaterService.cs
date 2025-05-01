using System.Reflection.Metadata.Ecma335;
using XChange.Data.Entities;
using XChange.Data.Repositories.CurrencyRate;

namespace XChange.Services;

public class CurrencyRateUpdaterService : ICurrencyRateUpdaterService
{
    private readonly ICurrencyRateRepository _currencyRateRepository;
    private decimal _forintCurrencyRate = 400;

    public CurrencyRateUpdaterService(ICurrencyRateRepository currencyRateRepository)
    {
        _currencyRateRepository = currencyRateRepository;
    }
    
    public async Task UpdateCurrencyRates(CancellationToken cancellationToken)
    {
        await Task.Delay(60 * 1000, cancellationToken);
            _forintCurrencyRate *= (decimal)1.01;
            CurrencyRateEntity newCurrencyRateEntity = new CurrencyRateEntity
            {
                CurrencyId = 1, Timestamp = DateTime.UtcNow, Rate = _forintCurrencyRate
            };

            await _currencyRateRepository.Create(newCurrencyRateEntity);
    }

    // public Task StartAsync(CancellationToken cancellationToken)
    // {
    //     _timer = new Timer(UpdateCurrencyRates, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    //
    //     return Task.CompletedTask;
    // }
    //
    // public Task StopAsync(CancellationToken cancellationToken)
    // {
    //     _timer?.Change(Timeout.Infinite, 0);
    //
    //     return Task.CompletedTask;
    // }
}