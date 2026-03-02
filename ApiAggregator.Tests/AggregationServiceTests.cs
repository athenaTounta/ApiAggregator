using ApiAggregator.Application.Abstractions;
using ApiAggregator.Application.Services;
using ApiAggregator.Domain.Models;
using FluentAssertions;
using FluentResults;
using Moq;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ApiAggregator.Tests
{
    public class AggregationServiceTests
    {
        private readonly Mock<IExternalApiService> _weatherServiceMock;
        private readonly Mock<IExternalApiService> _newsServiceMock;
        private readonly Mock<IExternalApiService> _gitHubServiceMock;
        private readonly AggregationService _sut;

        public AggregationServiceTests()
        {
            _weatherServiceMock = new Mock<IExternalApiService>();
            _newsServiceMock = new Mock<IExternalApiService>();
            _gitHubServiceMock = new Mock<IExternalApiService>();
            _sut = new AggregationService(
                  new[] { _weatherServiceMock.Object, _newsServiceMock.Object, _gitHubServiceMock.Object });
        }

        [Fact]
        public async Task GetDataAsync_WhenAllServicesSucceed_ReturnsAllItems()
        {
            var weatherItem = new AggregationItem { Title = "Athens - Clear", Category = "Weather", Date = DateTime.UtcNow };
            var newsItem = new AggregationItem { Title = "Breaking News", Category = "News", Date = DateTime.UtcNow };
            var githubItem = new AggregationItem { Title = "dotnet repo", Category = "GitHub", Date = DateTime.UtcNow };
            _weatherServiceMock.Setup(s => s.GetAsync())
            .ReturnsAsync(Result.Ok<IEnumerable<AggregationItem>>(new[] { weatherItem }));

            _newsServiceMock.Setup(s => s.GetAsync())
                .ReturnsAsync(Result.Ok<IEnumerable<AggregationItem>>(new[] { newsItem }));

            _gitHubServiceMock.Setup(s => s.GetAsync())
                .ReturnsAsync(Result.Ok<IEnumerable<AggregationItem>>(new[] { githubItem }));

            var result = await _sut.GetDataAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.AggregationItems.Should().HaveCount(3);
            result.Value.Errors.Should().BeEmpty();

        }
    }
}