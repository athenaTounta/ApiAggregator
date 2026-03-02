using ApiAggregator.Domain.Enums;

namespace ApiAggregator.Domain.Models
{
    public record AggregationItem
    {
        public string Title { get; init; } = String.Empty;
        public string Description { get; init; } = String.Empty;
        public string? Url { get; init; }
        public DateTime Date { get; init; }

        public string Category { get; init; } = String.Empty;
    }
}
