using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using RequestProcessingService.BusinessLogic.Models;
using RequestProcessingService.BusinessLogic.Services.Interfaces;
using RequestProcessingService.Infrastructure.Constants;
using RequestProcessingService.Infrastructure.Models;
using RequestProcessingService.Infrastructure.ServiceBus.Interfaces;

namespace RequestProcessingService.Infrastructure.ServiceBus;

public class ReportRequestEventHandler : IHandler<Ignore, ReportRequestEvent>
{
    private readonly ILogger<ReportRequestEventHandler> _logger;

    private readonly IReportRequestsService _reportRequestsService;

    public ReportRequestEventHandler
    (
        ILogger<ReportRequestEventHandler> logger,
        IReportRequestsService reportRequestsService
    )
    {
        _logger = logger;
        _reportRequestsService = reportRequestsService;
    }

    public async Task Handle
    (
        IReadOnlyCollection<ConsumeResult<Ignore, ReportRequestEvent>> messages,
        CancellationToken token
    )
    {
        var requests = messages
            .Select(x => x.Message.Value)
            .Select(x => new CreateReportRequestModel
            (
                new ConversionCheckPeriod
                (
                    x.EventConversionCheckPeriod.From,
                    x.EventConversionCheckPeriod.To
                ),
                x.ProductId,
                x.RequestId
            ))
            .ToArray();

        await _reportRequestsService.CreateReportRequests(requests, token);

        _logger.LogInformation(LogMessages.HandledCountMessages, messages.Count);
    }
}