using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaySecure.BLL.Services.IServices;

namespace StaySecure.PL.Areas.Admin
{
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(
            IReportService reportService)
        {
            _reportService = reportService;
        }

      

        [HttpGet("awareness")]
        public async Task<IActionResult>GetAwarenessReport()
        {
            var result =
                await _reportService
                    .GetAwarenessReportAsync();

            return Ok(result);
        }


    }
}
