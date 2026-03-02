using ApiAggregator.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ApiAggregator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : ControllerBase
    {
        private readonly IAggregationService _aggregationService;

        public AggregationController(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAggregationData()
        {
            var result = await _aggregationService.GetDataAsync();

            return Ok(result.Value);
        }
    }
}
