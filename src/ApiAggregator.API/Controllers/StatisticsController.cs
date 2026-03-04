using ApiAggregator.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ApiAggregator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public IActionResult GetStatistics()
        {
            var statistics = _statisticsService.GetStatistics();
            return Ok(statistics);
        }
    }
}
