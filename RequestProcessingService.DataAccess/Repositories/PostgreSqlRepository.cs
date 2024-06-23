using System.Transactions;
using Npgsql;
using RequestProcessingService.DataAccess.Configurations;
using RequestProcessingService.DataAccess.Repositories.Interfaces;

namespace RequestProcessingService.DataAccess.Repositories;

public abstract class PostgreSqlRepository : IPostgreSqlRepository
{
    private readonly DataAccessOptions _dataAccessSettings;

    protected const int DefaultTimeoutInSeconds = 5;

    protected PostgreSqlRepository(DataAccessOptions dataAccessSettings)
    {
        _dataAccessSettings = dataAccessSettings;
    }

    protected async Task<NpgsqlConnection> GetConnection()
    {
        if (Transaction.Current is not null &&
            Transaction.Current.TransactionInformation.Status is TransactionStatus.Aborted)
        {
            throw new TransactionAbortedException();
        }

        var connection = new NpgsqlConnection(_dataAccessSettings.PostgresConnectionString);
        await connection.OpenAsync();

        // Due to in-process migrations
        await connection.ReloadTypesAsync();

        return connection;
    }
}
