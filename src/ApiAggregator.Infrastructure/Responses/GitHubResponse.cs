using System.Text.Json.Serialization;

namespace ApiAggregator.Infrastructure.Responses;

public class GitHubResponse
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    public GitHubRepo[]? Items { get; set; }
}

public class GitHubRepo
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("stargazers_count")]
    public int StargazersCount { get; set; }

    public string? Language { get; set; }
}