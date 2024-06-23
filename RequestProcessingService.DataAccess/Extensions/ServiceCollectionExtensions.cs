using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RequestProcessingService.DataAccess.Configurations;
using RequestProcessingService.DataAccess.Repositories;
using RequestProcessingService.DataAccess.Repositories.Interfaces;

namespace RequestProcessingService.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess
    (
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.Configure<DataAccessOptions>(config.GetSection(nameof(DataAccessOptions)));
        
        Infrastructure.Postgres.MapCompositeTypes();
        Infrastructure.Postgres.AddMigrations(services);

        return services
            .AddTransient<IReportRequestsRepository, ReportRequestsRepository>()
            .AddTransient<ICachedReportResultsRepository, CachedReportResultsRepository>();
    }
}