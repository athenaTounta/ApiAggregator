using ApiAggregator.Domain.Models;
using FluentResults;
namespace ApiAggregator.Application.Abstractions
{
    public interface IExternalApiService
    {
        Task<Result<IEnumerable<AggregationItem>>> GetAsync();
    }
}
