namespace XChange.Services;

public class CurrencyRateUpdaterBackgroundService: BackgroundService
{
    private IServiceProvider Services;


    public CurrencyRateUpdaterBackgroundService(IServiceProvider services)
    {
        this.Services = services;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = Services.CreateScope();
        var scopedCurrencyRateUpdateService =
            scope.ServiceProvider.GetRequiredService<ICurrencyRateUpdaterService>();
        
        while (true)
        {
            await scopedCurrencyRateUpdateService.UpdateCurrencyRates(stoppingToken);
        }
    }
}