
using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.Enums;
using ApiAggregator.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace ApiAggregator.Infrastructure.ExternalClients
{
    public class WeatherService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _city;
        private readonly string _baseUrl;
        public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiAggregator");
            var weather = configuration.GetSection("ExternalApis:OpenWeather");
            _apiKey = weather["ApiKey"]!;
            _city = weather["City"]!;
            _baseUrl = weather["BaseUrl"]!;
        }
        //todo error handling: 1.problem with serialization 2.api down 3.no-key etc -->if rate limiting
        public async Task<IEnumerable<AggregationItem>> GetAsync()
        {
            var url = $"{_baseUrl}/weather?q={_city}&units=metric&appid={_apiKey}";
            var json = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<WeatherResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (result is null)
            {
                throw new InvalidOperationException("response is null");
            }
            return
            [
                new AggregationItem
            {
                Title = $"{result.Name} - {result.Weather?[0].Main}",
                Description = $"Temperature: {result.Main?.Temp}°C, Feels like: {result.Main?.FeelsLike}°C, Humidity: {result.Main?.Humidity}%",
                Url = null,
                Date = DateTimeOffset.FromUnixTimeSeconds(result.Dt).UtcDateTime,
                Category = ApiCategory.Weather
            }
            ];
        }
    }
}
