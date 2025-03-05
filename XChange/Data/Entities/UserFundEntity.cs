namespace XChange.Data.Entities;

public class UserFundEntity(int userId, int currencyId)
{
    public int Id;
    public int UserId = userId;
    public int CurrencyId = currencyId;
    public decimal Pending;
    public decimal Disposable;
}