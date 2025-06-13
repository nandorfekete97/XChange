namespace XChange.Models;

public record CurrencyRateModel(
    int Id,
    CurrencyModel CurrencyModel,
    decimal Rate);

