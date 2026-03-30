using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;

namespace StaySecure.PL.Areas.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ManagesController : ControllerBase
    {
        private readonly IManageUserService _ManageUserService;

        public ManagesController(IManageUserService ManageUserService)
        {
            _ManageUserService = ManageUserService;
        }
        [Authorize]
        [HttpGet("userDetails/{userId}")]
        public async Task<IActionResult> GetUserDetails([FromRoute] string userId)
        {
            var result = await _ManageUserService.GetUserDetailsAsync(userId);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _ManageUserService.GetUsersAsync();
            return Ok(result);
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



    }
}
