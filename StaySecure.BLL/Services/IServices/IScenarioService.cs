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
        Task<List<ScenarioListResponse>> GetAllScenariosAsync(AgeGroupEnum ageGroup, LevelEnum level, string lang);

        Task<BaseRespose> UpdateScenarioAsync(UpdateScenarioRequest request);

        Task<ScenarioPlayResponse?> GetNextScenarioAsync(string userId,string lang);

        Task<SubmitScenarioResponse> SubmitScenarioAsync(string userId, SubmitScenarioRequest request);

        Task<HintResponse?> GetHintAsync(string userId,int scenarioId);

        Task<ScenarioPlayResponse?> GetScenarioByIdAsync(int scenarioId, string lang);


        Task<List<ScenarioOverviewResponse>>GetScenarioOverviewAsync(string userId,string lang);
        Task<ScenarioPlayResponse?> GetScenarioByIdAsync(string userId,int scenarioId,string lang);




    }
}
