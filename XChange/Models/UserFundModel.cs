namespace XChange.Models;

public record UserFundModel(
    int Id, 
    CurrencyModel CurrencyModel, 
    decimal Pending, 
    decimal Disposable);
