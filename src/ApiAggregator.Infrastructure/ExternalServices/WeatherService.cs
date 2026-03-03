
using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.Models;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace ApiAggregator.Infrastructure.ExternalServices
{
    public class WeatherService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _city;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;
        public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration, JsonSerializerOptions jsonOptions)
        {
            _httpClient = httpClientFactory.CreateClient("ApiAggregator");
            var weather = configuration.GetSection("ExternalApis:OpenWeather");
            _apiKey = weather["ApiKey"]!;
            _city = weather["City"]!;
            _baseUrl = weather["BaseUrl"]!;
            _jsonOptions = jsonOptions;
        }

        public async Task<Result<IEnumerable<AggregationItem>>> GetAsync()
        {
            try
            {
                var url = $"{_baseUrl}/weather?q={_city}&units=metric&appid={_apiKey}";
                var json = await _httpClient.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<WeatherResponse>(json, _jsonOptions);
                if (result is null)
                {
                    return Result.Fail("Weather api result is null");
                }
                return Result.Ok<IEnumerable<AggregationItem>>(
                [
                    new AggregationItem
            {
                Title = $"{result.Name} - {result.Weather?[0].Main}",
                Description = $"Temperature: {result.Main?.Temp}°C, Feels like: {result.Main?.FeelsLike}°C, Humidity: {result.Main?.Humidity}%",
                Url = null,
                Date = DateTimeOffset.FromUnixTimeSeconds(result.Dt).UtcDateTime,
                Category = "Weather"
            }
                ]);
            }
            catch (HttpRequestException ex)
            {
                var message = ex.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Unauthorized access to weather API ",
                    HttpStatusCode.Forbidden => "Weather API access denied",
                    HttpStatusCode.TooManyRequests => "Weather API rate limit exceeded",
                    HttpStatusCode.ServiceUnavailable => "Weather API is down",
                    _ => $"Weather API failed: {ex.Message}"
                };
                return Result.Fail(message);
            }
            catch (JsonException ex)
            {
                return Result.Fail($"Weather API deserialization failed: {ex.Message}");
            }
        }
    }
}
