using Grpc.Core;
using RequestProcessingService.BusinessLogic.Services.Interfaces;

namespace RequestProcessingService.Presentation.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    private readonly IReportsService _reportsService;

    public GreeterService
    (
        ILogger<GreeterService> logger,
        IReportsService reportsService
    )
    {
        _logger = logger;
        _reportsService = reportsService;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = _reportsService.GetHello(request.Name)
        });
    }
}