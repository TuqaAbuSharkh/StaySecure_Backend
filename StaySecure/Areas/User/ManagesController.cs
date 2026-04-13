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
    [Authorize]

    public class ManagesController : ControllerBase
    {
        private readonly IManageUserService _ManageUserService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;



        public ManagesController(IManageUserService ManageUserService, IStringLocalizer<SharedResource> Localizer, UserManager<ApplicationUser> userManager)
        {
            _ManageUserService = ManageUserService;
            _localizer = Localizer;
            _userManager = userManager;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _ManageUserService.GetUserDetailsAsync(userId);

            if (result == null)
                return NotFound(new { message = _localizer["Error"].Value });

            return Ok(new
            {
                message = _localizer["Success"].Value,
                result
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _ManageUserService.UpdateProfileAsync(userId, request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


        [HttpGet("leaderboard/{userId}")]
        public async Task<IActionResult> GetLeaderboard(string userId)
        {
            var result = await _ManageUserService.GetLeaderboardAsync(userId);
            return Ok(result);
        }

        [HttpGet("progress")]
        public async Task<IActionResult> GetUserProgress()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var result = await _ManageUserService.GetUserProgressAsync(user.Id);

            return Ok(result);
        }
    }
}
