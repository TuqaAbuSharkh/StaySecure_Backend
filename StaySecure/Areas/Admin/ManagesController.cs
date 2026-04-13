using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.Models;
using StaySecure.PL.Resources;

namespace StaySecure.PL.Areas.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

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
  


        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _ManageUserService.GetUsersAsync();
            return Ok(new { message = _localizer["Success"].Value, result });
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

        [HttpPatch("block/{userId}")]
        public async Task<IActionResult> BlockUser([FromRoute] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Invalid userId");

            var result = await _ManageUserService.BlockedUserAsync(userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("unblock/{userId}")]
        public async Task<IActionResult> UnBlockUser([FromRoute] string userId)
        {
            var result = await _ManageUserService.UnBlockedUserAsync(userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            var result = await _ManageUserService.ChangeUserRoleAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpGet("LoginLogs/{userId}")]
        public async Task<IActionResult> GetLoginLogs([FromRoute] string userId)
        {
            
            var logs = await _ManageUserService.GetLoginLogsAsync(userId);

            return Ok(logs);
        }

        [HttpGet("AllLoginLogs")]
        public async Task<IActionResult> GetAllLoginLogs()
        {
            var logs = await _ManageUserService.GetAllLoginLogsAsync();
            return Ok(logs);
        }
    }
}
