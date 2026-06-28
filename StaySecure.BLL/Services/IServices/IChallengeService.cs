using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services.IServices
{
    public  interface IChallengeService
    {
        Task<ChallengeAccessResponse> GetChallengeAccessAsync(string userId);

        Task<ScenarioPlayResponse?> GetNextChallengeAsync(string userId);
        Task<ChallengeSubmitResponse> SubmitChallengeAsync(
    string userId,
    ChallengeSubmitRequest request);
    }
}
