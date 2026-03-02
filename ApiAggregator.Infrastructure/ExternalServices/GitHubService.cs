using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.Enums;
using ApiAggregator.Domain.Models;
using ApiAggregator.Infrastructure.Responses;
using FluentResults;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace ApiAggregator.Infrastructure.ExternalClients;

public class GitHubService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;
    public GitHubService(IHttpClientFactory httpClientFactory, IConfiguration configuration, JsonSerializerOptions jsonOptions)
    {
        _httpClient = httpClientFactory.CreateClient("ApiAggregator");
        _baseUrl = configuration.GetSection("ExternalApis:GitHub")["BaseUrl"]!;
        _jsonOptions = jsonOptions;
    }

    public async Task<Result<IEnumerable<AggregationItem>>> GetAsync()
    {
        try
        {
            var url = $"{_baseUrl}/search/repositories?q=dotnet&sort=stars";
            var json = await _httpClient.GetStringAsync(url);

            var result = JsonSerializer.Deserialize<GitHubResponse>(json, _jsonOptions);

            if (result is null || result.Items is null)
                return Result.Fail("GitHub API returned null response");

            return Result.Ok<IEnumerable<AggregationItem>>(
                result.Items.Select(r => new AggregationItem
                {
                    Title = r.Name ?? "No name",
                    Description = r.Description ?? "No description available",
                    Url = r.HtmlUrl,
                    Date = r.UpdatedAt,
                    Category = ApiCategory.GitHub
                }));
        }
        catch (HttpRequestException ex)
        {
            var message = ex.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Unauthorized access to gitHub API ",
                HttpStatusCode.Forbidden => "GitHub API access denied",
                HttpStatusCode.TooManyRequests => "GitHub API rate limit exceeded",
                HttpStatusCode.ServiceUnavailable => "GitHub API is down",
                _ => $"GitHub API failed: {ex.Message}"
            };
            return Result.Fail(message);
        }
        catch (JsonException ex)
        {
            return Result.Fail($"GitHub API deserialization failed: {ex.Message}");
        }
    }
}