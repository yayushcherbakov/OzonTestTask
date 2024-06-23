using RequestProcessingService.Access.Models;
using RequestProcessingService.Access.Services.Interfaces;

namespace RequestProcessingService.Access.Services;

internal class ReportsAccessService : IReportsAccessService
{
    /// <summary>
    /// Данный метод получает отчеты от внешнего сервиса.
    /// В ТЗ не было требования реализации сервиса геннерации отчетов.
    /// В ТЗ требовалось сократить количество запросов к данному сервису.
    /// Был определен контракт для взаимодействия с этим сервисом,
    /// но ответы этого сервиса генерируются случайным образом.
    /// </summary>
    public Task<IEnumerable<ReportResponsePayload>> GetReports
    (
        ReportRequestPayload[] reportRequestPayloads,
        CancellationToken cancellationToken
    )
    {
        var random = new Random();

        return Task.FromResult(reportRequestPayloads
            .Select(x =>
                new ReportResponsePayload
                (
                    x.RequestId,
                    random.NextDouble() * random.Next(minValue: 1, maxValue: int.MaxValue),
                    random.Next(minValue: 1, maxValue: int.MaxValue)
                )
            ));
    }
}