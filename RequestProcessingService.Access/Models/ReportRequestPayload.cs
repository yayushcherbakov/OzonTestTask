namespace RequestProcessingService.Access.Models;

public record ReportRequestPayload
(
    long RequestId,
    long ProductId,
    DateTimeOffset From,
    DateTimeOffset To
);