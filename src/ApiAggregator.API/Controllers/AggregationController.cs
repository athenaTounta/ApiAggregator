using ApiAggregator.Application.Abstractions;
using ApiAggregator.Domain.DTOs.Requests;
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
        public async Task<IActionResult> GetAggregationData([FromQuery] QueryParameters? parameters)
        {
            var result = await _aggregationService.GetDataAsync(parameters);

            if (result.IsFailed)
            {
                return BadRequest(new
                {
                    Errors = result.Errors.Select(e => e.Message)
                });
            }

            return Ok(result.Value);
        }
    }
}
