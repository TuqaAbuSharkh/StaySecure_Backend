using Microsoft.Extensions.Caching.Memory;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;
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
        private readonly IMemoryCache _cache;

        public ChallengeService(
            IChallengeRepository challengeRepository,
            IAiService aiService,
            IMemoryCache cache)
        {
            _challengeRepository = challengeRepository;
            _aiService = aiService;
            _cache = cache;
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
                throw new UnauthorizedAccessException(
                    "Complete all levels to unlock Challenges.");

            var aiChallenge = await _aiService.GenerateChallengeAsync();

            if (aiChallenge == null ||
                aiChallenge.Options == null ||
                aiChallenge.Options.Count != 4)
                return null;

            var challengeId =
                Random.Shared.Next(100000, 999999);

            var correctAnswerId =
                aiChallenge.Options
                    .Select((option, index) => new
                    {
                        option,
                        index
                    })
                    .First(x => x.option.IsCorrect)
                    .index + 1;

            _cache.Set(
                $"challenge_{userId}_{challengeId}",
                new ChallengeCacheDto
                {
                    Title = aiChallenge.Title,
                    Description = aiChallenge.Description,
                    Category = aiChallenge.Category,
                    CorrectAnswerId = correctAnswerId
                },
                TimeSpan.FromMinutes(30));

            return new ScenarioPlayResponse
            {
                Id = challengeId,

                Title = aiChallenge.Title,

                Description = aiChallenge.Description,

                Category = aiChallenge.Category,

                Score = 50,

                Hint = aiChallenge.Hint,


                Options = aiChallenge.Options
                    .Select((option, index) => new OptionResponse
                    {
                        Id = index + 1,
                        Text = option.Text
                    })
                    .ToList()
            };
        }

        public async Task<ChallengeSubmitResponse> SubmitChallengeAsync(
     string userId,
     ChallengeSubmitRequest request)
        {
            if (!_cache.TryGetValue(
                $"challenge_{userId}_{request.ChallengeId}",
                out ChallengeCacheDto challenge))
            {
                throw new Exception("Challenge expired.");
            }

            var isCorrect =
                request.SelectedOptionId ==
                challenge.CorrectAnswerId;

            var feedback =
                await _aiService.GenerateChallengeFeedbackAsync(
                    challenge.Title,
                    challenge.Description,
                    isCorrect);

            return new ChallengeSubmitResponse
            {
                IsCorrect = isCorrect,

                CorrectAnswerId =
                    challenge.CorrectAnswerId,

                EarnedScore =
                    isCorrect ? 50 : 0,

                Message =
                    isCorrect
                    ? "Correct!"
                    : "Incorrect.",

                Feedback = feedback
            };
        }


    }
}
