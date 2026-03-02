using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.DTOs.Responses;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ApiAggregator.Application.Decorators;

public class AggregationServiceDecorator : IAggregationService
{
    private readonly IAggregationService _aggregationService;
    private readonly ICacheService _cache;
    private readonly ILogger<AggregationServiceDecorator> _logger;
    private const string CacheKey = "AggregatedData";

    public AggregationServiceDecorator(
        IAggregationService aggregationService,
        ICacheService cache,
        ILogger<AggregationServiceDecorator> logger)
    {
        _aggregationService = aggregationService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<AggregationDataResponse>> GetDataAsync()
    {
        if (_cache.TryGet<AggregationDataResponse>(CacheKey, out var cached))
        {
            _logger.LogInformation("Cache returning cached data");
            return Result.Ok(cached);
        }

        _logger.LogInformation("Calling external api services");
        var result = await _aggregationService.GetDataAsync();

        if (result.IsSuccess)
            _cache.Set(CacheKey, result.Value);

        return result;
    }
}