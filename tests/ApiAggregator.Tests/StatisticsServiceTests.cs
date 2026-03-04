
using ApiAggregator.Application.Services;
using FluentAssertions;
using Xunit;

namespace ApiAggregator.Tests
{
    public class StatisticsServiceTests
    {
        [Fact]
        public async Task Record_WhenCalledFromMultipleThreads_DoesNotLoseData()
        {

            var service = new StatisticsService();
            var tasks = Enumerable.Range(0, 100)
                .Select(_ => Task.Run(() => service.RecordStatistics("Weather", 85)));

            await Task.WhenAll(tasks);

            var stats = service.GetStatistics().First();
            stats.TotalRequests.Should().Be(100);
        }

        [Fact]
        public void Record_WhenCalledOnce_RecordsCorrectly()
        {

            var service = new StatisticsService();

            service.RecordStatistics("Weather", 85);

            var stats = service.GetStatistics().First();
            stats.ApiName.Should().Be("Weather");
            stats.TotalRequests.Should().Be(1);
            stats.AverageResponseTime.Should().Be(85);
            stats.FastRequests.Should().Be(1);
            stats.AverageRequests.Should().Be(0);
            stats.SlowRequests.Should().Be(0);

        }

        [Fact]
        public void Record_WhenCalledWithMultipleAPIs_RecordsCorrectStatistics()
        {

            var service = new StatisticsService();
            service.RecordStatistics("Weather", 85);
            service.RecordStatistics("News", 123);
            service.RecordStatistics("GitHub", 210);

            var allStatistics = service.GetStatistics().ToList();

            var weather = allStatistics.First(s => s.ApiName == "Weather");
            weather.TotalRequests.Should().Be(1);
            weather.FastRequests.Should().Be(1);
            weather.AverageRequests.Should().Be(0);
            weather.SlowRequests.Should().Be(0);

            var news = allStatistics.First(s => s.ApiName == "News");
            news.TotalRequests.Should().Be(1);
            news.FastRequests.Should().Be(0);
            news.AverageRequests.Should().Be(1);
            news.SlowRequests.Should().Be(0);

            var gitHub = allStatistics.First(s => s.ApiName == "GitHub");
            gitHub.TotalRequests.Should().Be(1);
            gitHub.FastRequests.Should().Be(0);
            gitHub.AverageRequests.Should().Be(0);
            gitHub.SlowRequests.Should().Be(1);
        }

    }
}
