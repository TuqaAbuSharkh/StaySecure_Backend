using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;

namespace StaySecure.PL.Areas.Identity
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthinticationService _authenticationService;

        public AccountController(IAuthinticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authenticationService.RegisterAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);

        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);

        }

        [HttpGet("ConfirmEmail")] 
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var result = await _authenticationService.ConfirmEmailAsync(token, userId);

            if (!result)
                return BadRequest("Invalid or expired token");

            return Ok("Email confirmed successfully");
        }

        [HttpPost("Verify2FA")]
        public async Task<IActionResult> Verify2FA([FromBody] TwoFactorRequest request)
        {
            var result = await _authenticationService.VerifyTwoFactorAsync(request.UserId, request.Code);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Enable2FA")]
        public async Task<IActionResult> Enable2FA([FromQuery] string userId)
        {
            var result = await _authenticationService.Enable2FAAsync(userId);
            return Ok(result);
        }

        [HttpPost("Confirm2FA")]
        public async Task<IActionResult> Confirm2FA([FromBody] TwoFactorRequest request)
        {
            var result = await _authenticationService.Confirm2FAAsync(request.UserId, request.Code);

            if (result != "2FA Enabled Successfully")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("SendCode")]
        public async Task<IActionResult> RequestPassworsReset(ForgetPasswordRequest request)
        {
            var result = await _authenticationService.RequestPassworsReset(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPatch("PasswordReset")]
        public async Task<IActionResult> PassworsReset(ResetPasswordRequest request)
        {
            var result = await _authenticationService.ResetPassword(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPatch("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenApiModel request)
        {
            var result = await _authenticationService.RefreshTokenAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
