using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services.IServices
{
    public interface IAiService
    {
        Task<string> GenerateFeedbackAsync(string scenarioTitle,bool isCorrect);

        Task<AiScenarioDto?> GenerateScenarioAsync(List<string> weakTopics, AgeGroupEnum ageGroup, LevelEnum level);

        Task<string?> GenerateDailyTipAsync(string category, string lang);
        Task<AiChallengeDto?> GenerateChallengeAsync();
        Task<string?> GenerateChallengeFeedbackAsync(
            string title,
            string description,
            bool isCorrect);
    }
}
