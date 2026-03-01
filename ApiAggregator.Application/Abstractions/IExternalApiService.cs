using ApiAggregator.Domain.Models;

namespace ApiAggregator.Application.Abstractions
{
    public interface IExternalApiService
    {
        Task<IEnumerable<AggregationItem>> GetAsync();
    }
}
