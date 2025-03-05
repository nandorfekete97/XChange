namespace XChange.Data.Entities;

public class BookKeepingEntity(int exchangeInfoId, DateTime createdAt)
{
    public int Id;
    public int ExchangeInfoId = exchangeInfoId;
    public DateTime CreatedAt = createdAt;
}