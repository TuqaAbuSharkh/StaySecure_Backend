using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using StaySecure.DAL.Repositories.Interface;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services
{
    public class ScenarioService : IScenarioService
    {
        private readonly IScenarioRepository _scenarioRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAiService _aiService;

        public ScenarioService(IScenarioRepository scenarioRepository, UserManager<ApplicationUser> userManager,IAiService aiService)
        {
            _scenarioRepository = scenarioRepository;
            _userManager = userManager;
            _aiService = aiService;
        }

        public async Task<BaseRespose> CreateScenarioAsync(CreateScenarioRequest request)
        {
            var errors = new List<string>();

           

            if (request == null)
                errors.Add("Request is null");

            if (request?.Translations == null || !request.Translations.Any())
                errors.Add("Scenario must have at least one translation");

            if (request?.Options == null || !request.Options.Any())
                errors.Add("Scenario must have at least one option");

            if (request?.Options != null && !request.Options.Any(o => o.IsCorrect))
                errors.Add("At least one correct answer is required");

            if (request?.Options != null && request.Options.Count(o => o.IsCorrect) > 1)
                errors.Add("Only one correct answer is allowed");

            if (request?.Options != null && request.Options.Any(o => o.Translations == null || !o.Translations.Any()))
                errors.Add("Each option must have at least one translation");

            if (request?.Translations != null &&
                request.Translations.GroupBy(t => t.Language).Any(g => g.Count() > 1))
            {
                errors.Add("Duplicate languages in scenario translations");
            }

            
            if (errors.Any())
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                };
            }


            var scenario = new Scenario
            {
                AgeGroup = request.AgeGroup,
                Level = request.Level,
                Score = request.Score,
                CreatedAt = DateTime.UtcNow,
                Hint = request.Hint,
                HintPenalty = request.HintPenalty,

                Translations = request.Translations.Select(t => new ScenarioTranslation
                {
                    Title = t.Title,
                    Description = t.Description,
                    Category = t.Category,
                    Language = t.Language
                }).ToList(),

                Options = request.Options.Select(o => new ScenarioOption
                {
                    IsCorrect = o.IsCorrect,
                    Translations = o.Translations.Select(t => new ScenarioOptionTranslation
                    {
                        Text = t.Text,
                        Language = t.Language
                    }).ToList()
                }).ToList()
            };


            await _scenarioRepository.AddAsync(scenario);

            return new BaseRespose
            {
                Success = true,
                Message = "Scenario created successfully"
            };
        }


        public async Task<BaseRespose> DeleteScenarioAsync(int scenarioId)
        {
            var scenario = await _scenarioRepository.GetByIdAsync(scenarioId);

            if (scenario == null)
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "Scenario not found"
                };
            }

            await _scenarioRepository.DeleteAsync(scenario);

            return new BaseRespose
            {
                Success = true,
                Message = "Scenario deleted successfully"
            };
        }


        public async Task<List<ScenarioListResponse>> GetAllScenariosAsync(AgeGroupEnum ageGroup, LevelEnum level, string lang)
        {
            var scenarios = await _scenarioRepository.GetScenariosByAgeGroupAndLevelAsync(ageGroup, level);

            return scenarios.Select(s =>
            {
                var translation = s.Translations
                    .FirstOrDefault(t => t.Language == lang);

                return new ScenarioListResponse
                {
                    Id = s.Id,
                    AgeGroup = s.AgeGroup,
                    Level = s.Level,
                    Score = s.Score,
                    Title = translation?.Title ?? "",
                    Category = translation?.Category ?? ""
                };
            }).ToList();
        }

        public async Task<ScenarioPlayResponse?> GetNextScenarioAsync( string userId, string lang)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            var completedScenarios =
                await _scenarioRepository.GetUserScenariosAsync(userId);

            var scenario =
                await _scenarioRepository.GetNextScenarioAsync(
                    user.AgeGroup,
                    user.Level,
                    completedScenarios
                        .Select(x => x.ScenarioId)
                        .ToList());

            // خلصت السيناريوهات الثابتة
            if (scenario == null)
            {
                var requiredScore = 70;

                if (user.TotalScore >= requiredScore)
                {
                    if (user.Level != LevelEnum.Advanced)
                    {
                        user.Level++;

                        await _userManager.UpdateAsync(user);
                    }

                    return null;
                }

                // نقاط الضعف
                var weakTopics =
                    await _scenarioRepository
                        .GetTopWeakCategoriesAsync(userId);

                // توليد سيناريو من Gemini
                var aiScenario =
                    await _aiService.GenerateScenarioAsync(
                        weakTopics,
                        user.AgeGroup,
                        user.Level);

                if (aiScenario == null)
                    return null;

                // حفظ السيناريو المولد
                var generatedScenario = new Scenario
                {
                    AgeGroup = user.AgeGroup,
                    Level = user.Level,

                    IsAiGenerated = true,

                    Score = 5,
                    Hint = "Think carefully before acting.",
                    HintPenalty = 1,

                    Translations = new List<ScenarioTranslation>
            {
                new ScenarioTranslation
                {
                    Language = "en",
                    Title = aiScenario.Title,
                    Description = aiScenario.Description,
                    Category = aiScenario.Category
                }
            },

                    Options = aiScenario.Options.Select(x =>
                        new ScenarioOption
                        {
                            IsCorrect = x.IsCorrect,

                            Translations =
                            [
                                new ScenarioOptionTranslation
                        {
                            Language = "en",
                            Text = x.Text
                        }
                            ]
                        }).ToList()
                };

                await _scenarioRepository.AddAsync(
                    generatedScenario);

                return new ScenarioPlayResponse
                {
                    Id = generatedScenario.Id,

                    Title = aiScenario.Title,

                    Description = aiScenario.Description,

                    Category = aiScenario.Category,

                    Score = generatedScenario.Score,

                    Hint = generatedScenario.Hint,

                    HintPenalty = generatedScenario.HintPenalty,

                    Options = generatedScenario.Options
                        .Select(o => new OptionResponse
                        {
                            Id = o.Id,
                            Text = o.Translations.First().Text
                        })
                        .ToList()
                };
            }

            var translation = scenario.Translations
                .FirstOrDefault(x => x.Language == lang)
                ?? scenario.Translations.First();

            return new ScenarioPlayResponse
            {
                Id = scenario.Id,
                Title = translation.Title,
                Description = translation.Description,
                Category = translation.Category,
                Score = scenario.Score,
                Hint = scenario.Hint,
                HintPenalty = scenario.HintPenalty,

                Options = scenario.Options.Select(o =>
                {
                    var optionTranslation = o.Translations
                        .FirstOrDefault(x => x.Language == lang)
                        ?? o.Translations.First();

                    return new OptionResponse
                    {
                        Id = o.Id,
                        Text = optionTranslation.Text
                    };
                }).ToList()
            };
        }

        public async Task<SubmitScenarioResponse> SubmitScenarioAsync(string userId, SubmitScenarioRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new SubmitScenarioResponse
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            var scenario =
                await _scenarioRepository.GetByIdAsync(request.ScenarioId);

            if (scenario == null)
            {
                return new SubmitScenarioResponse
                {
                    Success = false,
                    Message = "Scenario not found"
                };
            }

            var option =
                await _scenarioRepository.GetOptionByIdAsync(
                    request.OptionId);

            if (option == null)
            {
                return new SubmitScenarioResponse
                {
                    Success = false,
                    Message = "Option not found"
                };
            }

            var isCorrect = option.IsCorrect;
            if (!isCorrect)
            {
                var category = scenario.Translations.First().Category;

                await _scenarioRepository.IncreaseWeakCategoryAsync(
                    userId,
                    category);
            }
            var title = scenario.Translations.FirstOrDefault()?.Title ?? "";
            string feedback = "";

            try
            {
                feedback = await _aiService.GenerateFeedbackAsync(
                    title,
                    isCorrect);
            }
            catch (Exception ex)
            {
                feedback = ex.ToString();
            }
            await _scenarioRepository.AddUserScenarioAsync(
                new UserScenario
                {
                    UserId = userId,
                    ScenarioId = scenario.Id,
                    IsCorrect = isCorrect,
                    CompletedAt = DateTime.UtcNow
                });

            if (isCorrect)
            {
                var earnedScore = scenario.Score;

                if (request.HintUsed)
                {
                    earnedScore -= scenario.HintPenalty;

                    if (earnedScore < 0)
                    {
                        earnedScore = 0;
                    }
                }

                user.TotalScore += earnedScore;

                await _userManager.UpdateAsync(user);
            }

            return new SubmitScenarioResponse
            {
                Success = true,
                Message = isCorrect
         ? "Correct answer"
         : "Wrong answer",
                IsCorrect = isCorrect,
                Feedback = feedback
            };
        }

        public async Task<BaseRespose> UpdateScenarioAsync(UpdateScenarioRequest request)
        {
            var errors = new List<string>();

            if (request == null)
                errors.Add("Request is null");

            if (request?.Translations == null || !request.Translations.Any())
                errors.Add("Scenario must have at least one translation");

            if (request?.Options == null || !request.Options.Any())
                errors.Add("Scenario must have at least one option");

            if (request?.Options != null && !request.Options.Any(o => o.IsCorrect))
                errors.Add("At least one correct answer is required");

            if (request?.Options != null && request.Options.Count(o => o.IsCorrect) > 1)
                errors.Add("Only one correct answer is allowed");

            if (request?.Options != null && request.Options.Any(o => o.Translations == null || !o.Translations.Any()))
                errors.Add("Each option must have at least one translation");

            if (errors.Any())
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                };
            }

            var scenario = await _scenarioRepository.GetByIdWithDetailsAsync(request.Id);

            if (scenario == null)
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "Scenario not found"
                };
            }

            scenario.Level = request.Level;
            scenario.Score = request.Score;
            scenario.Hint = request.Hint;
            scenario.HintPenalty = request.HintPenalty;

            scenario.Translations.Clear();
            scenario.Translations = request.Translations.Select(t => new ScenarioTranslation
            {
                Title = t.Title,
                Description = t.Description,
                Category = t.Category,
                Language = t.Language
            }).ToList();

            scenario.Options.Clear();
            scenario.Options = request.Options.Select(o => new ScenarioOption
            {
                IsCorrect = o.IsCorrect,
                Translations = o.Translations.Select(t => new ScenarioOptionTranslation
                {
                    Text = t.Text,
                    Language = t.Language
                }).ToList()
            }).ToList();

            await _scenarioRepository.UpdateAsync(scenario);

            return new BaseRespose
            {
                Success = true,
                Message = "Scenario updated successfully"
            };
        }

        public async Task<HintResponse?> GetHintAsync( string userId,int scenarioId)
        {
            var scenario =
                await _scenarioRepository.GetByIdAsync(
                    scenarioId);

            if (scenario == null)
                return null;

            return new HintResponse
            {
                Hint = scenario.Hint,
                HintPenalty = scenario.HintPenalty
            };
        }



    }
}
