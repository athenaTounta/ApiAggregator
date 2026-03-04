using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.Models;
using System.Collections.Concurrent;

namespace ApiAggregator.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<long>> _responseTimes = new();

        public IEnumerable<Statistics> GetStatistics()
        {
            return _responseTimes.Select(kvp =>
            {
                var times = kvp.Value.ToList();
                var total = times.Count;
                var average = total > 0 ? times.Average() : 0;

                return new Statistics
                {
                    ApiName = kvp.Key,
                    TotalRequests = total,
                    AverageResponseTime = Math.Round(average, 2),
                    FastRequests = times.Count(t => t < 100),
                    AverageRequests = times.Count(t => t >= 100 && t <= 200),
                    SlowRequests = times.Count(t => t > 200)
                };
            });
        }

        public void RecordStatistics(string apiName, long responseTime)
        {
            _responseTimes.AddOrUpdate(apiName, new ConcurrentBag<long> { responseTime }, (key, existing) =>
            {
                existing.Add(responseTime);
                return existing;
            });
        }
    }
}
