using FluentAssertions;
using Moq;
using RequestProcessingService.Access.Services.Interfaces;
using RequestProcessingService.BusinessLogic.Exceptions;
using RequestProcessingService.BusinessLogic.Models;
using RequestProcessingService.BusinessLogic.Services;
using RequestProcessingService.DataAccess.Entities;
using RequestProcessingService.DataAccess.Models;
using RequestProcessingService.DataAccess.Repositories.Interfaces;
using Xunit;

namespace RequestProcessingService.UnitTests;

public class ReportRequestsServiceTests
{
    private readonly Mock<IReportRequestsRepository> _reportRequestsRepository;
    private readonly Mock<ICachedReportResultsRepository> _cachedReportResultsRepository;
    private readonly Mock<IReportsAccessService> _reportsAccessService;

    public ReportRequestsServiceTests()
    {
        _reportRequestsRepository = new Mock<IReportRequestsRepository>();
        _cachedReportResultsRepository = new Mock<ICachedReportResultsRepository>();
        _reportsAccessService = new Mock<IReportsAccessService>();
    }

    [Fact]
    public async Task GetReportResult_CachedRequestId_ShouldReturnCompletedReportResultCache()
    {
        const int requestId = 1;

        var cachedReportResult = new CachedReportResult
        {
            IsCompleted = true,
            PaymentCount = 1,
            Racio = 0.1
        };

        _cachedReportResultsRepository.Setup(x =>
                x.GetCachedReportResult
                (
                    It.Is<long>(y => y == requestId),
                    It.IsAny<CancellationToken>())
            )
            .Returns(Task.FromResult(cachedReportResult)!);

        var service = new ReportRequestsService
        (
            _reportRequestsRepository.Object,
            _cachedReportResultsRepository.Object,
            _reportsAccessService.Object
        );

        var result = await service.GetReportResult(requestId, CancellationToken.None);

        Assert.NotNull(result);

        result.IsCompleted.Should().Be(true);
        result.Report.Should().NotBeNull();
        result.Report!.Racio.Should().Be(0.1);
        result.Report.PaymentCount.Should().Be(1);
    }

    [Fact]
    public async Task GetReportResult_CachedRequestId_ShouldReturnUncompletedReportResultCache()
    {
        const int requestId = 2;

        var cachedReportResult = new CachedReportResult
        {
            IsCompleted = false,
            PaymentCount = null,
            Racio = null
        };

        _cachedReportResultsRepository.Setup(x =>
                x.GetCachedReportResult
                (
                    It.Is<long>(y => y == requestId),
                    It.IsAny<CancellationToken>())
            )
            .Returns(Task.FromResult(cachedReportResult)!);

        var service = new ReportRequestsService
        (
            _reportRequestsRepository.Object,
            _cachedReportResultsRepository.Object,
            _reportsAccessService.Object
        );

        var result = await service.GetReportResult(requestId, CancellationToken.None);

        Assert.NotNull(result);

        result.IsCompleted.Should().Be(false);
        result.Report.Should().BeNull();
    }

    [Fact]
    public async Task GetReportResult_NotExistedRequestId_ShouldThrowNotFoundException()
    {
        const int requestId = 3;

        CachedReportResult? cachedReportResult = null;

        _cachedReportResultsRepository.Setup(x =>
                x.GetCachedReportResult(
                    It.Is<long>(y => y == requestId),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(cachedReportResult)!);

        _reportRequestsRepository.Setup(x =>
                x.Get(
                    It.Is<long[]>(y => y.Length == 1 && y[0] == requestId),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new ReportRequestEntityV1[] { })!);

        var service = new ReportRequestsService
        (
            _reportRequestsRepository.Object,
            _cachedReportResultsRepository.Object,
            _reportsAccessService.Object
        );

        Task Action()
        {
            return service.GetReportResult(requestId, CancellationToken.None);
        }

        await Assert.ThrowsAsync<NotFoundException>(Action);
    }

    [Fact]
    public async Task GetReportResult_ExistedRequestId_ShouldReturnCompletedReportResult()
    {
        const int requestId = 4;

        CachedReportResult? cachedReportResult = null;

        _cachedReportResultsRepository.Setup(x =>
                x.GetCachedReportResult(
                    It.Is<long>(y => y == requestId),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(cachedReportResult)!);

        _reportRequestsRepository.Setup(x =>
                x.Get(
                    It.Is<long[]>(y => y.Length == 1 && y[0] == requestId),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new ReportRequestEntityV1[]
            {
                new()
                {
                    RequestId = requestId,
                    IsCompleted = true,
                    ProductId = 1,
                    CheckPeriodFrom = DateTimeOffset.Now,
                    CheckPeriodTo = DateTimeOffset.Now,
                    Racio = 0.1,
                    PaymentCount = 1
                }
            })!);

        var service = new ReportRequestsService
        (
            _reportRequestsRepository.Object,
            _cachedReportResultsRepository.Object,
            _reportsAccessService.Object
        );

        var result = await service.GetReportResult(requestId, CancellationToken.None);

        Assert.NotNull(result);
        
        result.IsCompleted.Should().Be(true);
        result.Report.Should().NotBeNull();
        result.Report!.Racio.Should().Be(0.1);
        result.Report.PaymentCount.Should().Be(1);
    }
    
    [Fact]
    public async Task GetReportResult_ExistedRequestId_ShouldReturnUncompletedReportResult()
    {
        const int requestId = 5;

        CachedReportResult? cachedReportResult = null;

        _cachedReportResultsRepository.Setup(x =>
                x.GetCachedReportResult(
                    It.Is<long>(y => y == requestId),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(cachedReportResult)!);

        _reportRequestsRepository.Setup(x =>
                x.Get(
                    It.Is<long[]>(y => y.Length == 1 && y[0] == requestId),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new ReportRequestEntityV1[]
            {
                new()
                {
                    RequestId = requestId,
                    IsCompleted = false,
                    ProductId = 1,
                    CheckPeriodFrom = DateTimeOffset.Now,
                    CheckPeriodTo = DateTimeOffset.Now,
                    Racio = null,
                    PaymentCount = null
                }
            })!);

        var service = new ReportRequestsService
        (
            _reportRequestsRepository.Object,
            _cachedReportResultsRepository.Object,
            _reportsAccessService.Object
        );

        var result = await service.GetReportResult(requestId, CancellationToken.None);

        Assert.NotNull(result);

        result.IsCompleted.Should().Be(false);
        result.Report.Should().BeNull();
    }
    
    [Fact]
    public async Task CreateReportRequests_CreateReportRequestModels_ShouldNotThrowException()
    {
        var service = new ReportRequestsService
        (
            _reportRequestsRepository.Object,
            _cachedReportResultsRepository.Object,
            _reportsAccessService.Object
        );

        var requests = new CreateReportRequestModel[]
        {
            new CreateReportRequestModel
            (
                new ConversionCheckPeriod(DateTimeOffset.Now, DateTimeOffset.Now.AddMinutes(35)),
                1,
                2
            )
        };

        await service.CreateReportRequests(requests, CancellationToken.None);
    }

    [Fact]
    public async Task ProcessReportRequests_Nothing_ShouldNotThrowException()
    {
        var service = new ReportRequestsService
        (
            _reportRequestsRepository.Object,
            _cachedReportResultsRepository.Object,
            _reportsAccessService.Object
        );

        await service.ProcessReportRequests(CancellationToken.None);
    }
}