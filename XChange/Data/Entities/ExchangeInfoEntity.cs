using XChange.Models;

namespace XChange.Data.Entities;

public class ExchangeInfoEntity(
    int? id,
    DateTime? finalizedAt,
    DateTime startedAt,
    DateTime? failedAt,
    int userId,
    int sourceCurrencyId,
    int targetCurrencyId,
    int currencyRateId,
    decimal sourceCurrencyAmount,
    ExchangeStatus status,
    int? error)
{
    public int? Id = id;
    public DateTime? FinalizedAt = finalizedAt;
    public DateTime StartedAt = startedAt;
    public DateTime? FailedAt = failedAt;
    public int UserId = userId;
    public int SourceCurrencyId = sourceCurrencyId;
    public int TargetCurrencyId = targetCurrencyId;
    public int CurrencyRateId = currencyRateId;
    public decimal SourceCurrencyAmount = sourceCurrencyAmount;
    public ExchangeStatus Status = status;
    public int? Error = error;
}