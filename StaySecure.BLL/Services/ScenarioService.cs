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
                HintPenalty = request.HintPenalty,

                Translations = request.Translations.Select(t => new ScenarioTranslation
                {
                    Title = t.Title,
                    Description = t.Description,
                    Category = t.Category,
                    Language = t.Language,
                    Hint = t.Hint

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

        public async Task<ScenarioPlayResponse?> GetNextScenarioAsync(string userId, string lang)
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

            if (scenario == null)
            {
                var promoted =
                    await CheckLevelProgressionAsync(user);

                if (promoted)
                {
                    return await GetNextScenarioAsync(
                        user.Id,
                        lang);
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

                var generatedScenario = new Scenario
                {
                    AgeGroup = user.AgeGroup,

                    Level = user.Level,

                    IsAiGenerated = true,

                    Score = 5,

                    HintPenalty = 1,

                    Translations = new List<ScenarioTranslation>
            {
                new ScenarioTranslation
                {
                    Language = "en",
                    Title = aiScenario.Title,
                    Description = aiScenario.Description,
                    Category = aiScenario.Category,
                    Hint = aiScenario.Hint
                }
            },

                    Options = aiScenario.Options
                        .Select(x => new ScenarioOption
                        {
                            IsCorrect = x.IsCorrect,

                            Translations = new List<ScenarioOptionTranslation>
                            {
                        new ScenarioOptionTranslation
                        {
                            Language = "en",
                            Text = x.Text
                        }
                            }
                        })
                        .ToList()
                };

                await _scenarioRepository.AddAsync(generatedScenario);

                return new ScenarioPlayResponse
                {
                    Id = generatedScenario.Id,

                    Title = aiScenario.Title,

                    Description = aiScenario.Description,

                    Category = aiScenario.Category,

                    Score = generatedScenario.Score,

                    Hint = generatedScenario.Translations.First().Hint,

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

                Hint = translation.Hint,

                Score = scenario.Score,

                HintPenalty = scenario.HintPenalty,

                Options = scenario.Options
                    .Select(o =>
                    {
                        var optionTranslation = o.Translations
                            .FirstOrDefault(x => x.Language == lang)
                            ?? o.Translations.First();

                        return new OptionResponse
                        {
                            Id = o.Id,
                            Text = optionTranslation.Text
                        };
                    })
                    .ToList()
            };
        }


       
        private async Task<bool> CheckLevelProgressionAsync(ApplicationUser user)
        {
            var completedCount =
                await _scenarioRepository
                    .GetCompletedCountForLevelAsync(user.Id, user.Level);

            var usedHints =
                await _scenarioRepository
                    .GetUsedHintsCountForLevelAsync(user.Id, user.Level);

            if (user.Level == LevelEnum.Beginner)
            {
                if (completedCount >= 10 && usedHints <= 3)
                {
                    user.Level = LevelEnum.Intermediate;
                    await _userManager.UpdateAsync(user);
                    return true;
                }
            }

            if (user.Level == LevelEnum.Intermediate)
            {
                if (completedCount >= 10 && usedHints <= 5)
                {
                    user.Level = LevelEnum.Advanced;
                    await _userManager.UpdateAsync(user);
                    return true;
                }
            }

            return false;
        }

        public async Task<SubmitScenarioResponse> SubmitScenarioAsync(
            string userId,
            SubmitScenarioRequest request)
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

            var existingScenario =
                await _scenarioRepository.GetUserScenarioAsync(
                    userId,
                    request.ScenarioId);

            if (existingScenario != null)
            {
                return new SubmitScenarioResponse
                {
                    Success = true,
                    Message = "Scenario already completed.",
                    IsCorrect = existingScenario.IsCorrect,
                    Feedback = existingScenario.IsCorrect
                        ? "You have already completed this scenario successfully."
                        : "You have already attempted this scenario."
                };
            }

            var option =
                await _scenarioRepository.GetOptionByIdAsync(request.OptionId);

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
                        earnedScore = 0;
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
            scenario.AgeGroup = request.AgeGroup;
            scenario.Score = request.Score;
            scenario.HintPenalty = request.HintPenalty;

            scenario.Translations.Clear();
            scenario.Translations = request.Translations.Select(t => new ScenarioTranslation
            {
                Title = t.Title,
                Description = t.Description,
                Category = t.Category,
                Hint=t.Hint,
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
                Hint = scenario.Translations.First().Hint,
                HintPenalty = scenario.HintPenalty
            };
        }

        public async Task<ScenarioPlayResponse?> GetScenarioByIdAsync( int scenarioId,string lang)
        {
            var scenario =
                await _scenarioRepository.GetByIdAsync(
                    scenarioId);

            if (scenario == null)
                return null;

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
                Hint = translation.Hint,
                HintPenalty = scenario.HintPenalty,
                

                Options = scenario.Options.Select(o =>
                {
                    var optionTranslation = o.Translations
                        .FirstOrDefault(x => x.Language == lang)
                        ?? o.Translations.First();

                    return new OptionResponse
                    {
                        Id = o.Id,
                        Text = optionTranslation.Text,
                        IsCorrect = o.IsCorrect
                    };
                }).ToList()
            };
        }

        public async Task<List<ScenarioOverviewResponse>> GetScenarioOverviewAsync(string userId, string lang)
        {
            var user =await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new();

            var completed =await _scenarioRepository
                    .GetUserScenariosAsync(userId);

            var completedIds =
                completed
                    .Select(x => x.ScenarioId)
                    .ToHashSet();

            var scenarios =await _scenarioRepository
                    .GetScenariosByAgeGroupAsync(
                        user.AgeGroup,
                        user.Level);

            

            int nextUnlockedId =
                scenarios
                    .Where(x => !completedIds.Contains(x.Id))
                    .OrderBy(x => x.Id)
                    .Select(x => x.Id)
                    .FirstOrDefault();

            return scenarios.Select(x =>
            {
                var translation =
                    x.Translations
                        .FirstOrDefault(t => t.Language == lang)
                    ?? x.Translations.First();

                return new ScenarioOverviewResponse
                {
                    Id = x.Id,
                    Title = translation.Title,
                    Category = translation.Category,
                    Level = x.Level,
                    IsCompleted =
                        completedIds.Contains(x.Id),

                    IsUnlocked =
                        completedIds.Contains(x.Id)
                        || x.Id == nextUnlockedId
                };
            }).ToList();
        }

        public async Task<ScenarioPlayResponse?> GetScenarioByIdAsync(string userId, int scenarioId, string lang)
        {
            try
            {
                var user =
         await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return null;

                var scenario =
                    await _scenarioRepository
                        .GetScenarioByIdAsync(scenarioId);

                if (scenario == null)
                    return null;

                // يمنع الوصول لسيناريوهات Age Group أخرى
                if (scenario.AgeGroup != user.AgeGroup)
                    return null;

                var translation =
                    scenario.Translations
                        .FirstOrDefault(x => x.Language == lang)
                    ?? scenario.Translations.First();

                return new ScenarioPlayResponse
                {
                    Id = scenario.Id,
                    Title = translation.Title,
                    Description = translation.Description,
                    Category = translation.Category,
                    Score = scenario.Score,
                    Hint = translation.Hint,
                    HintPenalty = scenario.HintPenalty,

                    Options = scenario.Options
                        .Select(o =>
                        {
                            var optionTranslation =
                                o.Translations
                                    .FirstOrDefault(x => x.Language == lang)
                                ?? o.Translations.First();

                            return new OptionResponse
                            {
                                Id = o.Id,
                                Text = optionTranslation.Text
                            };
                        })
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                return new ScenarioPlayResponse
                {
                    Title = ex.Message
                };
            }


        }


    }
}
