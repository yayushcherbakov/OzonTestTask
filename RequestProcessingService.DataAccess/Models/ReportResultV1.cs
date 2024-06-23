namespace RequestProcessingService.DataAccess.Models;

public record ReportResultV1
(
    long RequestId,
    double? Racio,
    int? PaymentCount
);