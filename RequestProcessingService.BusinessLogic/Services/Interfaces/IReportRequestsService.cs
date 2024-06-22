using RequestProcessingService.BusinessLogic.Models;

namespace RequestProcessingService.BusinessLogic.Services.Interfaces;

public interface IReportRequestsService
{
    Task<ReportResult> GetReportResult(long requestId, CancellationToken cancellationToken);

    public Task CreateReportRequests(CreateReportRequestModel[] requests, CancellationToken cancellationToken);
}