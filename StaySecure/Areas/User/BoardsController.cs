using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.Models;
using StaySecure.PL.Resources;
using System.Security.Claims;

namespace StaySecure.PL.Areas.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]

    public class BoardsController : ControllerBase
    {

        private readonly IDashBoardService _dashBoardService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;



        public BoardsController(IDashBoardService DashBoardService, IStringLocalizer<SharedResource> Localizer, UserManager<ApplicationUser> userManager)
        {
         
            _dashBoardService = DashBoardService;
            _localizer = Localizer;
            _userManager = userManager;
        }

        [HttpGet("leaderboard")]
        [Authorize]
        public async Task<IActionResult> GetLeaderboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _dashBoardService.GetLeaderboardAsync(userId);

            return Ok(result);
        }

        [HttpGet("progress")]
        public async Task<IActionResult> GetUserProgress()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var result = await _dashBoardService.GetUserProgressAsync(user.Id);

            return Ok(result);
        }

    }
}
