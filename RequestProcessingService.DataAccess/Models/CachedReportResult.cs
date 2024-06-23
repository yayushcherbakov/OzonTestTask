namespace RequestProcessingService.DataAccess.Models;

public record CachedReportResult
{
    public bool IsCompleted { get; init; }

    public double? Racio { get; init; }

    public int? PaymentCount { get; init; }
}