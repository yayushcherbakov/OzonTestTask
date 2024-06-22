using System.Text.Json.Serialization;

namespace RequestProcessingService.Infrastructure.Models;

public class ReportRequestEvent
{
    [JsonPropertyName("event_conversion_check_period")]
    public EventConversionCheckPeriod EventConversionCheckPeriod { get; set; } = new();

    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    [JsonPropertyName("request_id")]
    public long RequestId { get; set; }
}