using RequestProcessingService.BusinessLogic.Constants;
using RequestProcessingService.BusinessLogic.Models;
using RequestProcessingService.BusinessLogic.Services.Interfaces;
using RequestProcessingService.DataAccess.Entities;
using RequestProcessingService.DataAccess.Repositories.Interfaces;

namespace RequestProcessingService.BusinessLogic.Services;

internal class ReportRequestsService : IReportRequestsService
{
    private readonly IReportRequestsRepository _reportRequestsRepository;

    public ReportRequestsService(IReportRequestsRepository reportRequestsRepository)
    {
        _reportRequestsRepository = reportRequestsRepository;
    }

    public async Task<ReportResult> GetReportResult(long requestId, CancellationToken cancellationToken)
    {
        var reportRequestEntities = await _reportRequestsRepository.Get(new[] { requestId }, cancellationToken);

        var reportRequestEntity = reportRequestEntities.SingleOrDefault();

        if (reportRequestEntity is null)
        {
            throw new ApplicationException(ErrorMessages.ReportRequestNotFound);
        }

        return new ReportResult
        (
            reportRequestEntity.IsCompleted,
            reportRequestEntity.IsCompleted
                ? new Report
                (
                    reportRequestEntity.Racio!.Value,
                    reportRequestEntity.PaymentCount!.Value
                )
                : null
        );
    }

    public async Task CreateReportRequests(CreateReportRequestModel[] requests, CancellationToken cancellationToken)
    {
        var requestEntities = requests
            .Select(x => new ReportRequestEntityV1()
            {
                RequestId = x.RequestId,
                IsCompleted = false,
                CheckPeriodFrom = x.ConversionCheckPeriod.From.ToUniversalTime(),
                CheckPeriodTo = x.ConversionCheckPeriod.To.ToUniversalTime(),
                ProductId = x.ProductId
            })
            .ToArray();

        await _reportRequestsRepository.Add(requestEntities, cancellationToken);
    }
}