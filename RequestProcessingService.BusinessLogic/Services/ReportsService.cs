using RequestProcessingService.BusinessLogic.Services.Interfaces;

namespace RequestProcessingService.BusinessLogic.Services;

internal class ReportsService : IReportsService
{
    public string GetHello(string name)
    {
        return "Hello " + name;
    }
}