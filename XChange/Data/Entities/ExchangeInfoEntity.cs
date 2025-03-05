using XChange.Models;

namespace XChange.Data.Entities;

public class ExchangeInfoEntity(
    DateTime startedAt,
    int userId,
    int sourceCurrencyId,
    int targetCurrencyId,
    int currencyRateId,
    decimal sourceCurrencyAmount,
    ExchangeStatus status)
{
    public int Id;
    public DateTime FinalizedAt;
    public DateTime StartedAt = startedAt;
    public DateTime FailedAt;
    public int UserId = userId;
    public int SourceCurrencyId = sourceCurrencyId;
    public int TargetCurrencyId = targetCurrencyId;
    public int CurrencyRateId = currencyRateId;
    public decimal SourceCurrencyAmount = sourceCurrencyAmount;
    public ExchangeStatus Status = status;
    public int? Error;
}