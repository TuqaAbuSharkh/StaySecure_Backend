using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;
using System.Security.Claims;

namespace StaySecure.PL.Areas.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengesController : ControllerBase
    {
        private readonly IChallengeService _challengeService;

        public ChallengesController(
            IChallengeService challengeService)
        {
            _challengeService = challengeService;
        }

        [HttpGet("access")]
        public async Task<IActionResult> GetAccess()
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)
                    ?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result =
                await _challengeService
                    .GetChallengeAccessAsync(userId);

            return Ok(result);
        }

        [HttpGet("next")]
        public async Task<IActionResult> GetNextChallenge()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var result = await _challengeService
                    .GetNextChallengeAsync(userId);

                if (result == null)
                    return BadRequest(new
                    {
                        message = "Unable to generate a challenge. Please try again."
                    });

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = ex.Message
                });
            }
        }



        [HttpPost("submit")]
        public async Task<IActionResult> SubmitChallenge(
    [FromBody] ChallengeSubmitRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var result = await _challengeService.SubmitChallengeAsync(userId, request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
