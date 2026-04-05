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
        //Leaderboard
        //public async Task<IActionResult> GetUsers([FromQuery] string lang="en")
        //{
        //    var result = await _ManageUserService.GetUsersAsync(lang);
        //    return Ok(new { message = _localizer["Success"].Value, result });
        //}

        [Authorize]
        [HttpGet("userDetails/{userId}")]
        public async Task<IActionResult> GetUserDetails([FromRoute] string userId )
        {
            var result = await _ManageUserService.GetUserDetailsAsync(userId);
            return Ok(new { message = _localizer["Success"].Value, result });
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _ManageUserService.GetUsersAsync();
            return Ok(new { message = _localizer["Success"].Value, result });
        }
       

        [HttpPatch("block/{userId}")]
        public async Task<IActionResult> BlockUser([FromRoute] string userId)
        => Ok(await _ManageUserService.BlockedUserAsync(userId));

        [HttpPatch("Unblock/{userId}")]
        public async Task<IActionResult> UnBlockUser([FromRoute] string userId)
        => Ok(await _ManageUserService.UnBlockedUserAsync(userId));

        [HttpPatch("change-role")]
        public async Task<IActionResult> ChangeUserRole(ChangeUserRoleRequest request)
        => Ok(await _ManageUserService.ChangeUserRoleAsync(request));



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
