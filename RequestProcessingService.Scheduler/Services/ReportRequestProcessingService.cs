using Microsoft.Extensions.Hosting;
using RequestProcessingService.BusinessLogic.Services.Interfaces;

namespace RequestProcessingService.Scheduler.Services;

public class ReportRequestProcessingService : BackgroundService
{
    private readonly IReportRequestsService _reportRequestsService;

    public ReportRequestProcessingService(IReportRequestsService reportRequestsService)
    {
        _reportRequestsService = reportRequestsService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var currentTime = DateTime.UtcNow;
            var nextRunTime = currentTime.Date.AddDays(1);
            var delay = nextRunTime - currentTime;

            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, cancellationToken);
            }

            await _reportRequestsService.ProcessReportRequests(cancellationToken);
        }
    }
}