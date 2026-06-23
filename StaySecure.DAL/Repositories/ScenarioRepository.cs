using Microsoft.EntityFrameworkCore;
using StaySecure.DAL.Data;
using StaySecure.DAL.Models;
using StaySecure.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Repositories
{
    public class ScenarioRepository : IScenarioRepository
    {
        private readonly ApplicationDbContext _context;

        public ScenarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Scenario scenario)
        {
            await _context.Scenarios.AddAsync(scenario);
            await _context.SaveChangesAsync();
        }

        public async Task<Scenario?> GetByIdAsync(int id)
        {
            return await _context.Scenarios
                .Include(s => s.Options)
                    .ThenInclude(o => o.Translations)
                .Include(s => s.Translations)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task DeleteAsync(Scenario scenario)
        {
            _context.Scenarios.Remove(scenario);
            await _context.SaveChangesAsync();
        }

        public async Task<IQueryable<Scenario>> GetQueryableAsync()
        {
            return _context.Scenarios
                .Include(s => s.Translations)
                .AsQueryable();
        }

        public async Task<Scenario?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Scenarios
                .Include(s => s.Translations)
                .Include(s => s.Options)
                    .ThenInclude(o => o.Translations)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Scenario scenario)
        {
            _context.Scenarios.Update(scenario);
            await _context.SaveChangesAsync();
        }


        public async Task<List<UserScenario>> GetUserScenariosAsync(string userId)
        {
            return await _context.UserScenarios
                .Where(u => u.UserId == userId)
                .ToListAsync();
        }

        public async Task<Scenario?> GetNextScenarioAsync(
    AgeGroupEnum ageGroup,
    LevelEnum level,
    List<int> completedScenarioIds)
        {
            return await _context.Scenarios
         .Include(x => x.Translations)
         .Include(x => x.Options)
             .ThenInclude(x => x.Translations)
         .FirstOrDefaultAsync(x =>
             x.AgeGroup == ageGroup &&
             x.Level == level &&
             !completedScenarioIds.Contains(x.Id));
        }

        public async Task<ScenarioOption?> GetOptionByIdAsync(int optionId)
        {
            return await _context.ScenarioOptions
        .FirstOrDefaultAsync(x => x.Id == optionId);
        }

        public async Task AddUserScenarioAsync(
    UserScenario userScenario)
        {
            await _context.UserScenarios.AddAsync(userScenario);

            await _context.SaveChangesAsync();
        }


       public async Task<List<Scenario>> GetScenariosByAgeGroupAndLevelAsync(AgeGroupEnum ageGroup, LevelEnum level)
        {
            return await _context.Scenarios
       .Include(x => x.Translations)
       .Where(x =>
           x.AgeGroup == ageGroup &&
           x.Level == level)
       .ToListAsync();
        }


        public async Task IncreaseWeakCategoryAsync( string userId,string category)
        {
            var weakCategory =
                await _context.UserWeakCategories
                    .FirstOrDefaultAsync(x =>
                        x.UserId == userId &&
                        x.Category == category);

            if (weakCategory == null)
            {
                weakCategory = new UserWeakCategory
                {
                    UserId = userId,
                    Category = category,
                    MistakeCount = 1
                };

                _context.UserWeakCategories.Add(weakCategory);
            }
            else
            {
                weakCategory.MistakeCount++;
            }

            await _context.SaveChangesAsync();
        }


        public async Task<List<string>> GetTopWeakCategoriesAsync(string userId)
        {
            return await _context.UserWeakCategories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.MistakeCount)
                .Take(3)
                .Select(x => x.Category)
                .ToListAsync();
        }

        public async Task<List<Scenario>> GetScenariosByAgeGroupAsync( AgeGroupEnum ageGroup, LevelEnum level)
        {
            return await _context.Scenarios
        .Include(x => x.Translations)
        .Where(x => x.AgeGroup == ageGroup)
        .OrderBy(x => x.Level)
        .ThenBy(x => x.Id)
        .ToListAsync();
        }

        public async Task<Scenario?> GetScenarioByIdAsync(int scenarioId)
        {
            return await _context.Scenarios
                .Include(x => x.Translations)
                .Include(x => x.Options)
                    .ThenInclude(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == scenarioId);
        }


        public async Task<int> GetCompletedCountForLevelAsync(string userId, LevelEnum level)
        {
            return await _context.UserScenarios
                .Join(
                    _context.Scenarios,
                    us => us.ScenarioId,
                    s => s.Id,
                    (us, s) => new { us, s })
                .Where(x => x.us.UserId == userId &&x.s.Level == level &&!x.s.IsAiGenerated)
                .CountAsync();
        }

        public async Task<int> GetUsedHintsCountForLevelAsync(string userId,LevelEnum level)
        {
            return await _context.UserScenarios
                .Join(
                    _context.Scenarios,
                    us => us.ScenarioId,
                    s => s.Id,
                    (us, s) => new { us, s })
                .Where(x =>x.us.UserId == userId &&
                     x.s.Level == level &&
                    !x.s.IsAiGenerated &&
                     x.us.HintUsed)
                .CountAsync();
        }


    }
}
