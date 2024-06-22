using System.Text.Json.Serialization;

namespace ReportRequestEventGenerator.Models;

public class EventConversionCheckPeriod
{
    [JsonPropertyName("from")]
    public DateTimeOffset From { get; set; }
    
    [JsonPropertyName("to")]
    public DateTimeOffset To { get; set; }
}
