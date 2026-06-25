using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Repositories.Interface;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services
{
    public  class ChallengeService :IChallengeService
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IAiService _aiService;

        public ChallengeService(
            IChallengeRepository challengeRepository,IAiService aiService)
        {
            _challengeRepository = challengeRepository;
            _aiService = aiService;
        }

        public async Task<ChallengeAccessResponse> GetChallengeAccessAsync(string userId)
        {
            var user =
                await _challengeRepository
                    .GetUserAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            return new ChallengeAccessResponse
            {
                IsUnlocked = user.ChallengesUnlocked
            };
        }

        public async Task<ScenarioPlayResponse?> GetNextChallengeAsync(string userId)
        {
            var user = await _challengeRepository.GetUserAsync(userId);

            if (user == null)
                return null;

            if (!user.ChallengesUnlocked)
                throw new UnauthorizedAccessException("Complete all levels to unlock Challenges.");

            var aiChallenge = await _aiService.GenerateChallengeAsync();

            if (aiChallenge == null ||
                aiChallenge.Options == null ||
                aiChallenge.Options.Count != 4)
                return null;

            return new ScenarioPlayResponse
            {
                Id = Random.Shared.Next(100000, 999999),

                Title = aiChallenge.Title,

                Description = aiChallenge.Description,

                Category = aiChallenge.Category,

                Score = 50,

                Hint = aiChallenge.Hint,
                HintPenalty = 5,

                Options = aiChallenge.Options
                    .Select((option, index) => new OptionResponse
                    {
                        Id = index + 1,
                        Text = option.Text
                    })
                    .ToList()
            };
        }

    }
}
