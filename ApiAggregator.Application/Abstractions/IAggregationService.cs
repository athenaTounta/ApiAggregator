using ApiAggregator.Domain.DTOs.Responses;

namespace ApiAggregator.Application.Abstractions
{
    public interface IAggregationService
    {
        Task<AggregationDataResponse> GetDataAsync();
    }
}
