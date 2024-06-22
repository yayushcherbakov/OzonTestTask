using RequestProcessingService.DataAccess.Entities;

namespace RequestProcessingService.DataAccess.Repositories.Interfaces;

public interface IReportRequestsRepository
{
    public Task<long[]> Add(ReportRequestEntityV1[] reportRequests, CancellationToken token);

    public Task<ReportRequestEntityV1[]> Get(long[] requestIds, CancellationToken token);

    public Task UpdateReportRequestResults(ReportRequestEntityV1[] reportRequests, CancellationToken token);
}