using XChange.Models;
using Microsoft.EntityFrameworkCore;

namespace XChange.Data.Entities;

[PrimaryKey(nameof(Id))]
public class ExchangeInfoEntity
{
    public int Id { get; set; } 
    public DateTime? FinalizedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public int UserId { get; set; }
    public int SourceCurrencyId { get; set; }
    public int TargetCurrencyId { get; set; }
    public int? CurrencyRateId { get; set; }
    public decimal SourceCurrencyAmount { get; set; }
    public ExchangeStatus Status { get; set; }
    public int? Error { get; set; }
    
    public ExchangeInfoEntity() {}
    
    public ExchangeInfoEntity(
        DateTime startedAt,
        int userId,
        int sourceCurrencyId,
        int targetCurrencyId,
        decimal sourceCurrencyAmount,
        ExchangeStatus status,
        DateTime? finalizedAt = null,
        DateTime? failedAt = null,
        int? currencyRateId = null,
        int? error = null)
    {
        StartedAt = startedAt;
        UserId = userId;
        SourceCurrencyId = sourceCurrencyId;
        TargetCurrencyId = targetCurrencyId;
        SourceCurrencyAmount = sourceCurrencyAmount;
        Status = status;
        FinalizedAt = finalizedAt;
        FailedAt = failedAt;
        CurrencyRateId = currencyRateId;
        Error = error;
    }
}