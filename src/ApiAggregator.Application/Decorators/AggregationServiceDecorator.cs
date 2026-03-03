using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.DTOs.Requests;
using ApiAggregator.Domain.DTOs.Responses;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ApiAggregator.Application.Decorators;

public class AggregationServiceDecorator : IAggregationService
{
    private readonly IAggregationService _aggregationService;
    private readonly ICacheService _cache;
    private readonly ILogger<AggregationServiceDecorator> _logger;

    public AggregationServiceDecorator(
        IAggregationService aggregationService,
        ICacheService cache,
        ILogger<AggregationServiceDecorator> logger)
    {
        _aggregationService = aggregationService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<AggregationDataResponse>> GetDataAsync(QueryParameters? parameters = null)
    {
        var category = parameters?.Category ?? "all";
        var sortBy = parameters?.SortBy ?? "none";
        var order = parameters?.Order?.ToString() ?? "none";

        var cacheKey = $"aggregated_{category}_{sortBy}_{order}";
        var staleCacheKey = $"stale_{cacheKey}";

        if (_cache.TryGet<AggregationDataResponse>(cacheKey, out var cached))
        {
            _logger.LogInformation("Cache returning cached data");
            return Result.Ok(cached);
        }

        _logger.LogInformation("Calling external api services");
        var result = await _aggregationService.GetDataAsync(parameters);

        if (result.IsSuccess)
        {
            _cache.Set(cacheKey, result.Value, TimeSpan.FromMinutes(5));
            _cache.Set(staleCacheKey, result.Value, TimeSpan.FromMinutes(30));
        }

        if (_cache.TryGet<AggregationDataResponse>(staleCacheKey, out var staleData))
        {
            _logger.LogWarning("Returning stale cached data ");
            return Result.Ok(staleData);
        }

        return result;
    }
}