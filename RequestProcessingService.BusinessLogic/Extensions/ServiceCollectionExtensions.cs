using Microsoft.Extensions.DependencyInjection;
using RequestProcessingService.BusinessLogic.Services;
using RequestProcessingService.BusinessLogic.Services.Interfaces;

namespace RequestProcessingService.BusinessLogic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
    {
        return services.AddTransient<IReportRequestsService, ReportRequestsService>();
    }
}