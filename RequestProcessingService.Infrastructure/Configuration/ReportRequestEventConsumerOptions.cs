namespace RequestProcessingService.Infrastructure.Configuration;

public class ReportRequestEventConsumerOptions
{
    public string BootstrapServers { get; set; } = string.Empty;

    public string GroupId { get; set; } = string.Empty;

    public string ReportRequestEvents { get; set; } = string.Empty;

    public int ChannelCapacity { get; set; }

    public int BufferDelayInSeconds { get; set; }

    public int MaxRetryAttempts { get; set; }

    public int RetryDelayInMilliseconds { get; set; }
}