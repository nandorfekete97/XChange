namespace XChange.Models;

public class UserFundModel(int id, CurrencyModel currencyModel, decimal pending, decimal disposable)
{
    public int Id = id;
    public CurrencyModel CurrencyModel = currencyModel;
    public decimal Pending = pending;
    public decimal Disposable = disposable;
}