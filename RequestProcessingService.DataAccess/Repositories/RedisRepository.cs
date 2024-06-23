using RequestProcessingService.DataAccess.Configurations;
using RequestProcessingService.DataAccess.Repositories.Interfaces;
using StackExchange.Redis;

namespace RequestProcessingService.DataAccess.Repositories;

internal abstract class RedisRepository : IRedisRepository
{
    private static ConnectionMultiplexer? _connection;

    private readonly DataAccessOptions _dataAccessOptions;

    protected RedisRepository(DataAccessOptions dataAccessOptions)
    {
        _dataAccessOptions = dataAccessOptions;
    }

    protected abstract string KeyPrefix { get; }

    protected virtual TimeSpan ExpirePeriod => TimeSpan.MaxValue;

    protected async Task<IDatabase> GetConnection()
    {
        _connection ??= await ConnectionMultiplexer.ConnectAsync(_dataAccessOptions.RedisConnectionString);

        return _connection.GetDatabase();
    }

    protected RedisKey GetKey(params object[] identifiers)
        => new($"{KeyPrefix}:{string.Join(':', identifiers)}");
}