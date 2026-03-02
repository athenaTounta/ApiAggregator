using ApiAggregator.Domain.Models;

namespace ApiAggregator.Domain.DTOs.Responses
{
    public class AggregationDataResponse
    {
        public IEnumerable<AggregationItem> AggregationItems { get; set; } = [];
        public int TotalCount { get; set; }
        public IEnumerable<string> Errors { get; set; } = [];
    }
}
