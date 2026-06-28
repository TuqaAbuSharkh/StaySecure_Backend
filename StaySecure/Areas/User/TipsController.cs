using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaySecure.BLL.Services.IServices;
using System.Security.Claims;

namespace StaySecure.PL.Areas.User
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TipsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public TipsController(
            IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyTip([FromQuery] string lang = "en")
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _reportService.GetDailyTipAsync(userId, lang);

            return Ok(result);
        }


    }
}
