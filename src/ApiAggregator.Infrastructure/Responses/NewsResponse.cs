
public class NewsResponse
{
    public string? Status { get; set; }
    public int TotalResults { get; set; }
    public Article[]? Articles { get; set; }
}

public class Article
{
    public NewsSource? Source { get; set; }
    public string? Author { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public DateTime PublishedAt { get; set; }
}

public class NewsSource
{
    public string Name { get; set; } = String.Empty;
}
