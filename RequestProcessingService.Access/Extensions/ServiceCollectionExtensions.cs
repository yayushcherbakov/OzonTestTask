using Microsoft.Extensions.DependencyInjection;
using RequestProcessingService.Access.Services;
using RequestProcessingService.Access.Services.Interfaces;

namespace RequestProcessingService.Access.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccessServices(this IServiceCollection services)
    {
        return services.AddTransient<IReportsAccessService, ReportsAccessService>();
    }
}