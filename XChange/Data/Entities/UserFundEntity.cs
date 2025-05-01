namespace XChange.Data.Entities;

public class UserFundEntity
{
    public int Id { get; set; } 
    public int UserId { get; set; } 
    public int CurrencyId { get; set; }
    public decimal Pending { get; set; }
    public decimal Disposable { get; set; }

    public UserFundEntity()
    {
    }

    public UserFundEntity(int userId, int currencyId, decimal pending, decimal disposable)
    {
        UserId = userId;
        CurrencyId = currencyId;
        Pending = pending;
        Disposable = disposable;
    }
}
