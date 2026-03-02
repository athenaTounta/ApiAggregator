
using ApiAggregator.Domain.Enums;

namespace ApiAggregator.Domain.DTOs.Requests
{
    public record QueryParameters
    {
        public string? Category { get; init; }
        public string? SortBy { get; init; }
        public SortOrder? Order { get; init; }
    }
}
