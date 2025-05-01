namespace XChange.Data.Entities;

public class BookKeepingEntity
{
    public int Id { get; set; }
    public int ExchangeInfoId { get; set; }
    public DateTime CreatedAt { get; set; }

    public BookKeepingEntity() { }

    public BookKeepingEntity(int exchangeInfoId, DateTime createdAt)
    {
        ExchangeInfoId = exchangeInfoId;
        CreatedAt = createdAt;
    }
}