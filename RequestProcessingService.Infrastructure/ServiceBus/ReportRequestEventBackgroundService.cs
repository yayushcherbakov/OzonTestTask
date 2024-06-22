using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RequestProcessingService.Infrastructure.Configuration;
using RequestProcessingService.Infrastructure.Constants;
using RequestProcessingService.Infrastructure.Helpers;
using RequestProcessingService.Infrastructure.Models;

namespace RequestProcessingService.Infrastructure.ServiceBus;

public class ReportRequestEventBackgroundService : BackgroundService
{
    private readonly ReportRequestEventConsumer<Ignore, ReportRequestEvent> _consumer;
    private readonly ILogger<ReportRequestEventBackgroundService> _logger;

    public ReportRequestEventBackgroundService
    (
        IServiceProvider serviceProvider,
        ILogger<ReportRequestEventBackgroundService> logger,
        IOptions<ReportRequestEventConsumerOptions> options
    )
    {
        _logger = logger;

        var handler = serviceProvider.GetRequiredService<ReportRequestEventHandler>();

        var kafkaConsumerOptions = options.Value;

        _consumer = new ReportRequestEventConsumer<Ignore, ReportRequestEvent>(
            handler,
            kafkaConsumerOptions.BootstrapServers,
            kafkaConsumerOptions.GroupId,
            kafkaConsumerOptions.ReportRequestEvents,
            null,
            new SystemTextJsonDeserializer<ReportRequestEvent>(),
            serviceProvider.GetRequiredService<ILogger<ReportRequestEventConsumer<Ignore, ReportRequestEvent>>>(),
            options);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Dispose();

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _consumer.Consume(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogMessages.UnhandledErrorMessage);
        }
    }
}