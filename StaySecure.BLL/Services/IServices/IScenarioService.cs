using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services.IServices
{
    public interface IScenarioService
    {
        Task<BaseRespose> CreateScenarioAsync(CreateScenarioRequest request);
        Task<BaseRespose> DeleteScenarioAsync(int scenarioId);
        Task<PagedResponse<ScenarioListResponse>> GetAllScenariosAsync(GetScenariosRequest request);

        Task<BaseRespose> UpdateScenarioAsync(UpdateScenarioRequest request);
        

        }
}
