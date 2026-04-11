using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.Models;

namespace StaySecure.PL.Areas.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ScenariosController : ControllerBase
    {
        private readonly IScenarioService _scenarioService;

        public ScenariosController(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        
    }
}
