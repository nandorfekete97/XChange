namespace XChange.Models;

public class CurrencyRateModel(CurrencyModel currencyModel, decimal rate)
{
    public CurrencyModel CurrencyModel = currencyModel;
    public decimal Rate = rate;
}