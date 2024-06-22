namespace RequestProcessingService.BusinessLogic.Models;

public record CreateReportRequestModel
(
    ConversionCheckPeriod ConversionCheckPeriod,
    long ProductId,
    long RequestId
);