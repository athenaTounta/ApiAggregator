using ApiAggregator.Application.Abstractions;
using ApiAggregator.Application.Decorators;
using ApiAggregator.Domain.DTOs.Responses;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiAggregator.Tests;

public class AggregationServiceDecoratorTests
{
    private readonly Mock<IAggregationService> _aggregationServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<AggregationServiceDecorator>> _loggerMock;
    private readonly AggregationServiceDecorator _sut;

    public AggregationServiceDecoratorTests()
    {
        _aggregationServiceMock = new Mock<IAggregationService>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<AggregationServiceDecorator>>();

        _sut = new AggregationServiceDecorator(
            _aggregationServiceMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetDataAsync_WhenCacheHit_ReturnsFromCacheWithoutCallingService()
    {

        var cachedResponse = new AggregationDataResponse
        {
            AggregationItems = [],
            TotalCount = 0
        };

        _cacheServiceMock
            .Setup(c => c.TryGet<AggregationDataResponse>(It.IsAny<string>(), out cachedResponse))
            .Returns(true);

        var result = await _sut.GetDataAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(cachedResponse);
        _aggregationServiceMock.Verify(s => s.GetDataAsync(null), Times.Never);
    }

    [Fact]
    public async Task GetDataAsync_WhenAllApisFail_ReturnsStaleData()
    {
        var staleResponse = new AggregationDataResponse
        {
            AggregationItems = [new() { Title = "Stale item", Category = "Weather", Date = DateTime.UtcNow }],
            TotalCount = 1
        };

        AggregationDataResponse? fresh = null;
        AggregationDataResponse? stale = staleResponse;

        _cacheServiceMock
            .Setup(c => c.TryGet<AggregationDataResponse>(
                It.Is<string>(k => k.StartsWith("aggregated_")), out fresh))
            .Returns(false);

        _cacheServiceMock
            .Setup(c => c.TryGet<AggregationDataResponse>(
                It.Is<string>(k => k.StartsWith("stale_")), out stale))
            .Returns(true);

        _aggregationServiceMock
            .Setup(s => s.GetDataAsync(null))
            .ReturnsAsync(Result.Fail<AggregationDataResponse>("All apis failed"));

        var result = await _sut.GetDataAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(1);
        result.Value.AggregationItems.First().Title.Should().Be("Stale item");
    }
}
