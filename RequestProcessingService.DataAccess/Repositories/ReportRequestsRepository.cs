using Dapper;
using Microsoft.Extensions.Options;
using RequestProcessingService.DataAccess.Configurations;
using RequestProcessingService.DataAccess.Entities;
using RequestProcessingService.DataAccess.Models;
using RequestProcessingService.DataAccess.Repositories.Interfaces;

namespace RequestProcessingService.DataAccess.Repositories;

internal sealed class ReportRequestsRepository : PostgreSqlRepository, IReportRequestsRepository
{
    public ReportRequestsRepository(IOptions<DataAccessOptions> dataAccessSettings) : base(dataAccessSettings.Value)
    {
    }

    public async Task<long[]> Add(ReportRequestEntityV1[] reportRequests, CancellationToken token)
    {
        const string sqlQuery = @"
   insert into report_requests (request_id
        , is_completed
        , product_id
        , check_period_from
        , check_period_to
        , racio
        , payment_count)
   select request_id
        , is_completed
        , product_id
        , check_period_from
        , check_period_to
        , racio
        , payment_count
     from UNNEST(@ReportRequests)
returning request_id;
";

        await using var connection = await GetConnection();

        var ids = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    ReportRequests = reportRequests
                },
                cancellationToken: token));

        return ids.ToArray();
    }

    public async Task<ReportRequestEntityV1[]> Get(long[] requestIds, CancellationToken token)
    {
        var baseSql = @"
select request_id
     , is_completed
     , product_id
     , check_period_from
     , check_period_to
     , racio
     , payment_count
 from report_requests
";

        var conditions = new List<string>();
        var @params = new DynamicParameters();

        if (requestIds.Any())
        {
            conditions.Add($"request_id = ANY(@RequestIds)");
            @params.Add($"RequestIds", requestIds);
        }

        var cmd = new CommandDefinition(
            baseSql + $" where {string.Join(" and ", conditions)} ",
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);

        await using var connection = await GetConnection();

        return (await connection.QueryAsync<ReportRequestEntityV1>(cmd)).ToArray();
    }

    public async Task UpdateReportRequestResults(ReportResultV1[] reportResults, CancellationToken token)
    {
        const string sqlQuery = @"
update report_requests rr
   set is_completed = true
     , racio = nrr.racio
     , payment_count = nrr.payment_count
  from UNNEST(@ReportResults) nrr
 where nrr.request_id = rr.request_id
   and rr.is_completed = false
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    ReportResults = reportResults
                },
                cancellationToken: token));
    }

    public async Task<ReportRequestEntityV1[]> GetIncompleteReportRequests(CancellationToken token)
    {
        var baseSql = @"
select request_id
     , is_completed
     , product_id
     , check_period_from
     , check_period_to
     , racio
     , payment_count
 from report_requests
where is_completed = false
";
        var @params = new DynamicParameters();

        var cmd = new CommandDefinition(
            baseSql,
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);

        await using var connection = await GetConnection();

        return (await connection.QueryAsync<ReportRequestEntityV1>(cmd)).ToArray();
    }
}