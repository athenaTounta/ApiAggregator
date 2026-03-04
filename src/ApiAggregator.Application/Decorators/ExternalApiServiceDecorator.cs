using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.Models;
using FluentResults;
using System.Diagnostics;

namespace ApiAggregator.Infrastructure.Decorators;

public class ExternalApiServiceDecorator : IExternalApiService
{
    private readonly IExternalApiService _externalApiService;
    private readonly IStatisticsService _statisticsService;

    public ExternalApiServiceDecorator(
        IExternalApiService inner,
        IStatisticsService statisticsService)
    {
        _externalApiService = inner;
        _statisticsService = statisticsService;
    }

    public async Task<Result<IEnumerable<AggregationItem>>> GetAsync()
    {
        var apiName = _externalApiService.GetType().Name.Replace("Service", "");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            return await _externalApiService.GetAsync();
        }
        finally
        {
            stopwatch.Stop();
            _statisticsService.RecordStatistics(apiName, stopwatch.ElapsedMilliseconds);
        }
    }
}