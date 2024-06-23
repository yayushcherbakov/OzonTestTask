using RequestProcessingService.DataAccess.Models;

namespace RequestProcessingService.DataAccess.Repositories.Interfaces;

public interface ICachedReportResultsRepository
{
    Task<CachedReportResult?> GetCachedReportResult(long requestId, CancellationToken token);

    Task Add(long requestId, CachedReportResult model, CancellationToken token);

    Task Delete(long requestId, CancellationToken token);
}