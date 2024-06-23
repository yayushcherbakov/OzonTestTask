using RequestProcessingService.DataAccess.Entities;
using RequestProcessingService.DataAccess.Models;

namespace RequestProcessingService.DataAccess.Repositories.Interfaces;

public interface IReportRequestsRepository : IPostgreSqlRepository
{
    Task<long[]> Add(ReportRequestEntityV1[] reportRequests, CancellationToken token);

    Task<ReportRequestEntityV1[]> Get(long[] requestIds, CancellationToken token);

    Task UpdateReportRequestResults(ReportResultV1[] reportResults, CancellationToken token);

    Task<ReportRequestEntityV1[]> GetIncompleteReportRequests(CancellationToken token);
}