
using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.DTOs.Responses;

namespace ApiAggregator.Application.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IEnumerable<IExternalApiService> _externalApiServices;

        public AggregationService(IEnumerable<IExternalApiService> externalApiServices)
        {
            _externalApiServices = externalApiServices;
        }

        public async Task<AggregationDataResponse> GetDataAsync()
        {
            var servicesTasks = _externalApiServices.Select(service => service.GetAsync());
            var results = await Task.WhenAll(servicesTasks);
            var aggregationItems = results.SelectMany(result => result).ToList();
            return new AggregationDataResponse
            {
                AggregationItems = aggregationItems,
                TotalCount = aggregationItems.Count
            };
        }
    }
}
