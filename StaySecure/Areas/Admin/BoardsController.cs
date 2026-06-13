using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaySecure.BLL.Services.IServices;

namespace StaySecure.PL.Areas.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class BoardsController : ControllerBase
    {
        private readonly IDashBoardService _dashBoardService;

        public BoardsController(
            IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var result =
                await _dashBoardService
                    .GetAdminLeaderboardAsync();

            return Ok(result);
        }
    }
}
