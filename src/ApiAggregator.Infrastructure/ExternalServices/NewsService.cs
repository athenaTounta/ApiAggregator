
using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.Enums;
using ApiAggregator.Domain.Models;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace ApiAggregator.Infrastructure.ExternalServices
{
    public class NewsService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public NewsService(IHttpClientFactory httpClientFactory, IConfiguration configuration, JsonSerializerOptions jsonOptions)
        {
            _httpClient = httpClientFactory.CreateClient("ApiAggregator");
            var news = configuration.GetSection("ExternalApis:News");
            _apiKey = news["ApiKey"]!;
            _baseUrl = news["BaseUrl"]!;
            _jsonOptions = jsonOptions;
        }

        public async Task<Result<IEnumerable<AggregationItem>>> GetAsync()
        {
            try
            {
                var url = $"{_baseUrl}/top-headlines?country=us&apiKey={_apiKey}";
                var json = await _httpClient.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<NewsResponse>(json, _jsonOptions);
                if (result is null || result.Articles is null)
                {
                    return Result.Fail("News api result is null");
                }
                return Result.Ok<IEnumerable<AggregationItem>>(
                result.Articles
                 .Where(a => a.Title != null)
                 .Select(a => new AggregationItem
                 {
                     Title = a.Title!,
                     Description = a.Description ?? "No description available",
                     Url = a.Url,
                     Date = a.PublishedAt,
                     Category = "News"
                 }));
            }
            catch (HttpRequestException ex)
            {
                var message = ex.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Unauthorized access to news API ",
                    HttpStatusCode.Forbidden => "News API access denied",
                    HttpStatusCode.TooManyRequests => "News API rate limit exceeded",
                    HttpStatusCode.ServiceUnavailable => "News API is down",
                    _ => $"News API failed: {ex.Message}"
                };
                return Result.Fail(message);
            }
            catch (JsonException ex)
            {
                return Result.Fail($"News API deserialization failed: {ex.Message}");
            }
        }
    }
}
