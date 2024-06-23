using RequestProcessingService.Access.Models;
using RequestProcessingService.Access.Services.Interfaces;
using RequestProcessingService.BusinessLogic.Constants;
using RequestProcessingService.BusinessLogic.Exceptions;
using RequestProcessingService.BusinessLogic.Models;
using RequestProcessingService.BusinessLogic.Services.Interfaces;
using RequestProcessingService.DataAccess.Entities;
using RequestProcessingService.DataAccess.Models;
using RequestProcessingService.DataAccess.Repositories.Interfaces;

namespace RequestProcessingService.BusinessLogic.Services;

internal class ReportRequestsService : IReportRequestsService
{
    private readonly IReportRequestsRepository _reportRequestsRepository;
    private readonly ICachedReportResultsRepository _cachedReportResultsRepository;
    private readonly IReportsAccessService _reportsAccessService;

    public ReportRequestsService
    (
        IReportRequestsRepository reportRequestsRepository,
        ICachedReportResultsRepository cachedReportResultsRepository,
        IReportsAccessService reportsAccessService
    )
    {
        _reportRequestsRepository = reportRequestsRepository;
        _cachedReportResultsRepository = cachedReportResultsRepository;
        _reportsAccessService = reportsAccessService;
    }

    public async Task<ReportResult> GetReportResult(long requestId, CancellationToken cancellationToken)
    {
        var cache = await _cachedReportResultsRepository.GetCachedReportResult(requestId, cancellationToken);

        if (cache is not null)
        {
            return new ReportResult
            (
                cache.IsCompleted,
                cache.IsCompleted
                    ? new Report
                    (
                        cache.Racio!.Value,
                        cache.PaymentCount!.Value
                    )
                    : null);
        }

        var reportRequestEntities = await _reportRequestsRepository.Get
        (
            new[] { requestId },
            cancellationToken
        );

        var reportRequestEntity = reportRequestEntities.SingleOrDefault();

        if (reportRequestEntity is null)
        {
            throw new NotFoundException(ErrorMessages.ReportRequestNotFound);
        }

        await _cachedReportResultsRepository.Add
        (
            requestId,
            new CachedReportResult()
            {
                IsCompleted = reportRequestEntity.IsCompleted,
                Racio = reportRequestEntity.Racio,
                PaymentCount = reportRequestEntity.PaymentCount
            },
            cancellationToken
        );

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

    public async Task ProcessReportRequests(CancellationToken cancellationToken)
    {
        var incompleteReportRequests = await _reportRequestsRepository.GetIncompleteReportRequests(cancellationToken);

        var reportRequests = incompleteReportRequests
            .Select(x =>
                new ReportRequestPayload
                (
                    x.RequestId,
                    x.ProductId,
                    x.CheckPeriodFrom,
                    x.CheckPeriodTo
                )
            )
            .ToArray();

        var reports = await _reportsAccessService.GetReports(reportRequests, cancellationToken);

        var reportRequestEntitiesV1 = reports
            .Select(x =>
                new ReportResultV1
                (
                    x.RequestId,
                    x.Racio,
                    x.PaymentCount
                )
            )
            .ToArray();

        await _reportRequestsRepository.UpdateReportRequestResults(reportRequestEntitiesV1, cancellationToken);
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