using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RequestProcessingService.Infrastructure.Configuration;
using RequestProcessingService.Infrastructure.ServiceBus;

namespace RequestProcessingService.Infrastructure.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddReportRequestEventHandler(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<ReportRequestEventConsumerOptions>(config.GetSection(nameof(ReportRequestEventConsumerOptions)));

        services.AddSingleton<ReportRequestEventHandler>();

        services.AddHostedService<ReportRequestEventBackgroundService>();

        return services;
    }
}