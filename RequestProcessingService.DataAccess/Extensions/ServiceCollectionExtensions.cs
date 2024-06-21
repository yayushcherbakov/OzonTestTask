using Microsoft.Extensions.DependencyInjection;
using RequestProcessingService.DataAccess.Repositories;
using RequestProcessingService.DataAccess.Repositories.Interfaces;

namespace RequestProcessingService.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        return services.AddScoped<IReportRequestsRepository, ReportRequestsRepository>();
    }
}