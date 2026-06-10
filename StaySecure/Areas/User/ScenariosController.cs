using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.Models;
using System.Security.Claims;

namespace StaySecure.PL.Areas.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class ScenariosController : ControllerBase
    {
        private readonly IScenarioService _scenarioService;

        public ScenariosController(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        [HttpGet("next")]
        public async Task<IActionResult> GetNextScenario(
    string lang = "en")
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result =
                await _scenarioService.GetNextScenarioAsync(
                    userId,
                    lang);

            return Ok(result);
        }


        [HttpPost("submit")]
        public async Task<IActionResult> SubmitScenario(SubmitScenarioRequest request)
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result =
                await _scenarioService.SubmitScenarioAsync(
                    userId,
                    request);

            return Ok(result);
        }


       


    }
}
