namespace RequestProcessingService.DataAccess.Configurations;

public record DataAccessOptions
{
    public string PostgresConnectionString { get; init; } = string.Empty;
}
