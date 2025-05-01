namespace XChange.Data.Entities;

public class CurrencyRateEntity
{
    public int Id { get; set; }
    public int CurrencyId { get; set; }
    public decimal Rate { get; set; }
    public DateTime Timestamp { get; set; }

    public CurrencyRateEntity() { }

    public CurrencyRateEntity(int currencyId, decimal rate, DateTime timestamp)
    {
        CurrencyId = currencyId;
        Rate = rate;
        Timestamp = timestamp;
    }
}