using Microsoft.EntityFrameworkCore;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using StaySecure.DAL.Repositories.Interface;
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

        public ScenarioService(IScenarioRepository scenarioRepository)
        {
            _scenarioRepository = scenarioRepository;
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
                Difficulty = request.Difficulty,
                Score = request.Score,
                CreatedAt = DateTime.UtcNow,

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


        public async Task<PagedResponse<ScenarioListResponse>> GetAllScenariosAsync(GetScenariosRequest request)
        {
            var query = await _scenarioRepository.GetQueryableAsync();

            if (request.Difficulty.HasValue)
                query = query.Where(s => s.Difficulty == request.Difficulty.Value);

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(s =>
                    s.Translations.Any(t => t.Title.Contains(request.Search)));
            }

            var totalCount = await query.CountAsync();

            var scenarios = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var data = scenarios.Select(s =>
            {
                var translation = s.Translations
                    .FirstOrDefault(t => t.Language == request.Lang);

                return new ScenarioListResponse
                {
                    Id = s.Id,
                    Difficulty = s.Difficulty,
                    Score = s.Score,
                    Title = translation?.Title ?? "No Title",
                    Category = translation?.Category ?? "No Category"
                };
            }).ToList();

            return new PagedResponse<ScenarioListResponse>
            {
                Data = data,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
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

            scenario.Difficulty = request.Difficulty;
            scenario.Score = request.Score;

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


      

    }
}
