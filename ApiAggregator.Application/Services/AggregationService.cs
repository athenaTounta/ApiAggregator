
using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.DTOs.Responses;
using ApiAggregator.Domain.Models;
using FluentResults;

namespace ApiAggregator.Application.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IEnumerable<IExternalApiService> _externalApiServices;

        public AggregationService(IEnumerable<IExternalApiService> externalApiServices)
        {
            _externalApiServices = externalApiServices;
        }

        public async Task<Result<AggregationDataResponse>> GetDataAsync()
        {
            var servicesTasks = _externalApiServices.Select(service => service.GetAsync());
            var results = await Task.WhenAll(servicesTasks);
            var failedErrors = results
    .Where(r => r.IsFailed)
    .SelectMany(r => r.Errors)
    .Select(e => e.Message)
    .ToList();
            var aggregationItems = results
     .Where(r => r.IsSuccess)
     .SelectMany(r => r.Value ?? Enumerable.Empty<AggregationItem>())
     .ToList();
            return Result.Ok(new AggregationDataResponse
            {
                AggregationItems = aggregationItems,
                TotalCount = aggregationItems.Count,
                Errors = failedErrors
            });
        }
    }
}
