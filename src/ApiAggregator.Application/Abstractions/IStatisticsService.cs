using ApiAggregator.Domain.Models;

namespace ApiAggregator.Application.Abstractions
{
    public interface IStatisticsService
    {
        void RecordStatistics(string apiName, long responseTime);
        IEnumerable<Statistics> GetStatistics();

    }
}