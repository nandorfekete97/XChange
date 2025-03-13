namespace XChange.Data.Entities;

public class UserFundEntity(int id, int userId, int currencyId, decimal pending, decimal disposable)
{
    public int Id = id;
    public int UserId = userId;
    public int CurrencyId = currencyId;
    public decimal Pending = pending;
    public decimal Disposable = disposable;
}
