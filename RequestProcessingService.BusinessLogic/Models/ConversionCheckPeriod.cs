namespace RequestProcessingService.BusinessLogic.Models;

public record ConversionCheckPeriod
(
    DateTimeOffset From,
    DateTimeOffset To
);