using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.Enums;
using ApiAggregator.Domain.Models;
using ApiAggregator.Infrastructure.Responses;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ApiAggregator.Infrastructure.ExternalClients;

public class GitHubService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    public GitHubService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("ApiAggregator");
        var github = configuration.GetSection("ExternalApis:GitHub");
        _baseUrl = github["BaseUrl"]!;
    }

    public async Task<IEnumerable<AggregationItem>> GetAsync()
    {
        var url = $"{_baseUrl}/search/repositories?q=dotnet&sort=stars";
        var json = await _httpClient.GetStringAsync(url);
        var result = JsonSerializer.Deserialize<GitHubResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (result is null || result.Items is null)
        {
            throw new InvalidOperationException("response is null");
        }
        return result.Items
    .Select(r => new AggregationItem
    {
        Title = r.Name ?? "No name",
        Description = r.Description ?? "No description available",
        Url = r.HtmlUrl,
        Date = r.UpdatedAt,
        Category = ApiCategory.GitHub
    });
    }
}