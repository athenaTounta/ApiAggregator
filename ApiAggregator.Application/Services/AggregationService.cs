
using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.DTOs.Requests;
using ApiAggregator.Domain.DTOs.Responses;
using ApiAggregator.Domain.Enums;
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

        public async Task<Result<AggregationDataResponse>> GetDataAsync(QueryParameters? parameters = null)
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

            if (aggregationItems.Count == 0 && failedErrors.Count > 0)
            {
                return Result.Fail("All apis failed")
                    .WithErrors(failedErrors);
            }

            var query = aggregationItems.AsQueryable();

            if (parameters is not null)
            {
                query = ApplyFilters(query, parameters);
                query = ApplySorting(query, parameters);
            }

            var items = query.ToList();

            return Result.Ok(new AggregationDataResponse
            {
                AggregationItems = items,
                TotalCount = items.Count,
                Errors = failedErrors
            });

        }
        private static IQueryable<AggregationItem> ApplyFilters(IQueryable<AggregationItem> query, QueryParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.Category))
                query = query.Where(i => i.Category.Equals(parameters.Category, StringComparison.OrdinalIgnoreCase));

            return query;
        }

        private static IQueryable<AggregationItem> ApplySorting(IQueryable<AggregationItem> query, QueryParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.SortBy))
                return query;

            return parameters.SortBy.ToLower() switch
            {
                "date" => parameters.Order == SortOrder.Desc
                    ? query.OrderByDescending(i => i.Date)
                    : query.OrderBy(i => i.Date),
                "title" => parameters.Order == SortOrder.Desc
                    ? query.OrderByDescending(i => i.Title)
                    : query.OrderBy(i => i.Title),
                _ => query
            };
        }
    }
}

