using Grpc.Core;
using ReportRequestsGrpc;
using RequestProcessingService.BusinessLogic.Services.Interfaces;

namespace RequestProcessingService.Presentation.Services;

public class ReportRequestsGrpcService : ReportRequestsService.ReportRequestsServiceBase
{
    private readonly ILogger<ReportRequestsGrpcService> _logger;
    private readonly IReportRequestsService _reportRequestsService;

    public ReportRequestsGrpcService
    (
        ILogger<ReportRequestsGrpcService> logger,
        IReportRequestsService reportRequestsService
    )
    {
        _logger = logger;
        _reportRequestsService = reportRequestsService;
    }

    public override async Task<GetReportResultResponse> GetReportResult
    (
        GetReportResultRequest request,
        ServerCallContext context
    )
    {
        var reportResult = await _reportRequestsService.GetReportResult(request.RequestId, context.CancellationToken);

        var response = new GetReportResultResponse()
        {
            IsCompleted = reportResult.IsCompleted,
            Report = reportResult.Report is null
                ? null
                : new GetReportResultResponse.Types.Report()
                {
                    Racio = reportResult.Report.Racio,
                    PaymentCount = reportResult.Report.PaymentCount
                }
        };

        return response;
    }
}