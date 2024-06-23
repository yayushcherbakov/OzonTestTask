using RequestProcessingService.Access.Models;

namespace RequestProcessingService.Access.Services.Interfaces;

public interface IReportsAccessService
{
    Task<IEnumerable<ReportResponsePayload>> GetReports
    (
        ReportRequestPayload[] reportRequestPayloads,
        CancellationToken cancellationToken
    );
}