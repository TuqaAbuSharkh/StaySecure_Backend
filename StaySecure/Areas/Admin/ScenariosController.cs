using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using StaySecure.PL.Resources;

namespace StaySecure.PL.Areas.Admin
{
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class ScenariosController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IScenarioService _scenarioService;

        public ScenariosController( IStringLocalizer<SharedResource> Localizer,IScenarioService scenarioService)
        {
            _localizer = Localizer;
            _scenarioService = scenarioService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateScenario([FromBody] CreateScenarioRequest request)
        {
            var response = await _scenarioService.CreateScenarioAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScenario([FromRoute]int id)
        {
            var response = await _scenarioService.DeleteScenarioAsync(id);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllScenarios([FromQuery] GetScenariosRequest request)
        {
            var result = await _scenarioService.GetAllScenariosAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateScenario([FromBody] UpdateScenarioRequest request)
        {
            var response = await _scenarioService.UpdateScenarioAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

    }
}
