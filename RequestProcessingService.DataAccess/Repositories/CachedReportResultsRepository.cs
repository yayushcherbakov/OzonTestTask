using Microsoft.Extensions.Options;
using RequestProcessingService.DataAccess.Configurations;
using RequestProcessingService.DataAccess.Models;
using RequestProcessingService.DataAccess.Repositories.Interfaces;
using StackExchange.Redis;

namespace RequestProcessingService.DataAccess.Repositories;

internal sealed class CachedReportResultsRepository : RedisRepository, ICachedReportResultsRepository
{
    protected override TimeSpan ExpirePeriod => TimeSpan.FromMinutes(5);

    protected override string KeyPrefix => "report_results";

    public CachedReportResultsRepository(IOptions<DataAccessOptions> dataAccessSettings)
        : base(dataAccessSettings.Value)
    {
    }

    public async Task<CachedReportResult?> GetCachedReportResult(long requestId, CancellationToken token)
    {
        var connection = await GetConnection();

        var key = GetKey(requestId);
        var fields = await connection.HashGetAllAsync(key);

        if (!fields.Any())
        {
            return null;
        }

        var result = new CachedReportResult();
        foreach (var field in fields)
        {
            if (!field.Value.HasValue)
            {
                continue;
            }

            switch (field.Name.ToString())
            {
                case "is_completed":
                {
                    if (!field.Value.TryParse(out int value))
                    {
                        return null;
                    }

                    result = result with { IsCompleted = value != 0 };

                    break;
                }
                case "racio":
                {
                    if (!field.Value.TryParse(out int value))
                    {
                        return null;
                    }

                    result = result with { Racio = value };

                    break;
                }
                case "payment_count":
                {
                    if (!field.Value.TryParse(out int value))
                    {
                        return null;
                    }

                    result = result with { PaymentCount = value };

                    break;
                }
                default:
                    return null;
            }
        }

        return result;
    }

    public async Task Add(long requestId, CachedReportResult model, CancellationToken token)
    {
        var connection = await GetConnection();

        var key = GetKey(requestId);

        var hashEntries = new List<HashEntry>
        {
            new("is_completed", model.IsCompleted)
        };

        if (model.Racio is not null)
        {
            hashEntries.Add(new HashEntry("racio", model.Racio));
        }

        if (model.PaymentCount is not null)
        {
            hashEntries.Add(new HashEntry("payment_count", model.PaymentCount));
        }

        await connection.HashSetAsync(key, hashEntries.ToArray());

        await connection.KeyExpireAsync(key, ExpirePeriod);
    }

    public async Task Delete(long requestId, CancellationToken token)
    {
        var connection = await GetConnection();

        var key = GetKey(requestId);
        await connection.KeyDeleteAsync(key);
    }
}