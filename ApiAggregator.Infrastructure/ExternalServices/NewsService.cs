
using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.Enums;
using ApiAggregator.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ApiAggregator.Infrastructure.ExternalClients
{
    public class NewsService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public NewsService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiAggregator");
            var news = configuration.GetSection("ExternalApis:News");
            _apiKey = news["ApiKey"]!;
            _baseUrl = news["BaseUrl"]!;
        }

        public async Task<IEnumerable<AggregationItem>> GetAsync()
        {
            var url = $"{_baseUrl}/top-headlines?country=us&apiKey={_apiKey}";
            var json = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<NewsResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (result is null || result.Articles is null)
            {
                throw new InvalidOperationException("response is null");
            }
            return result.Articles
                .Where(a => a.Title != null)
                .Select(a => new AggregationItem
                {
                    Title = a.Title!,
                    Description = a.Description ?? "No description available",
                    Url = a.Url,
                    Date = a.PublishedAt,
                    Category = ApiCategory.News
                });
        }
    }
}
