using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.Models;
using StaySecure.PL.Resources;

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

        [HttpGet("userDetails/{userId}")]
        public async Task<IActionResult> GetUserDetails([FromRoute] string userId)
        {
            var currentUserId = User.FindFirst("uid")?.Value;

            if (currentUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var result = await _ManageUserService.GetUserDetailsAsync(userId);

            if (result == null)
                return NotFound(new { message = _localizer["Error"].Value });

            return Ok(new
            {
                message = _localizer["Success"].Value,
                result
            });
        }


        [HttpGet("leaderboard/{userId}")]
        public async Task<IActionResult> GetLeaderboard(string userId)
        {
            var result = await _ManageUserService.GetLeaderboardAsync(userId);
            return Ok(result);
        }
    }
}
