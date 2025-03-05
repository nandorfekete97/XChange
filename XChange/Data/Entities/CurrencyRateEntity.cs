namespace XChange.Data.Entities;

public class CurrencyRateEntity(int currencyId, decimal rate, DateTime timestamp)
{
    public int Id;
    public int CurrencyId = currencyId;
    public decimal Rate = rate;
    public DateTime Timestamp = timestamp;
}