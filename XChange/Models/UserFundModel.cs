namespace XChange.Models;

public record UserFundModel(
    int Id, 
    int UserId,
    CurrencyModel CurrencyModel, 
    decimal Pending, 
    decimal Disposable);
