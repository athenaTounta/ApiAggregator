using ApiAggregator.Domain.DTOs.Requests;
using ApiAggregator.Domain.DTOs.Responses;
using FluentResults;

namespace ApiAggregator.Application.Abstractions
{
    public interface IAggregationService
    {
        Task<Result<AggregationDataResponse>> GetDataAsync(QueryParameters? parameters = null);
    }
}
