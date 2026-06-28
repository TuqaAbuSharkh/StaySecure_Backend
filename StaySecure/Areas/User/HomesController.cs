using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaySecure.BLL.Services.IServices;

namespace StaySecure.PL.Areas.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomesController : ControllerBase
    {
        private readonly IReportService _reportService;

        public HomesController(
            IReportService reportService)
        {
            _reportService = reportService;
        }


        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _reportService.GetHomeStatisticsAsync();

            return Ok(result);
        }
    }
}
