using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregator.Domain.Models
{
    public class Statistics
    {
        public string ApiName { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public double AverageResponseTime { get; set; }
        public int FastRequests { get; set; }
        public int AverageRequests { get; set; }
        public int SlowRequests { get; set; }
    }
}
