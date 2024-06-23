using RequestProcessingService.BusinessLogic.Models;

namespace RequestProcessingService.BusinessLogic.Services.Interfaces;

public interface IReportRequestsService
{
    Task<ReportResult> GetReportResult(long requestId, CancellationToken cancellationToken);

    Task CreateReportRequests(CreateReportRequestModel[] requests, CancellationToken cancellationToken);

    Task ProcessReportRequests(CancellationToken cancellationToken);
}