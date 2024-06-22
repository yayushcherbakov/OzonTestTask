namespace RequestProcessingService.DataAccess.Entities;

public record ReportRequestEntityV1
{
    public long RequestId { get; set; }

    public bool IsCompleted { get; set; }

    public long ProductId { get; set; }

    public DateTimeOffset CheckPeriodFrom { get; set; }

    public DateTimeOffset CheckPeriodTo { get; set; }
    
    public double? Racio { get; set; }
    
    public int? PaymentCount { get; set; }
}