using Microsoft.Extensions.DependencyInjection;
using RequestProcessingService.Scheduler.Services;

namespace RequestProcessingService.Scheduler.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScheduler(this IServiceCollection services)
    {
        return services.AddHostedService<ReportRequestProcessingService>();
    }
}