using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Repositories.Interface
{
    public interface IScenarioRepository
    {
        Task AddAsync(Scenario scenario);

        Task<Scenario?> GetByIdAsync(int id);
        Task DeleteAsync(Scenario scenario);

        Task<IQueryable<Scenario>> GetQueryableAsync();
        Task<Scenario?> GetByIdWithDetailsAsync(int id);
        Task UpdateAsync(Scenario scenario);
        Task<List<UserScenario>> GetUserScenariosAsync(string userId);

        Task<Scenario?> GetNextScenarioAsync( AgeGroupEnum ageGroup, LevelEnum level, List<int> completedScenarioIds);

        Task<ScenarioOption?> GetOptionByIdAsync(int optionId);

        Task AddUserScenarioAsync(UserScenario userScenario);

        Task<List<Scenario>> GetScenariosByAgeGroupAndLevelAsync( AgeGroupEnum ageGroup, LevelEnum level);


        Task IncreaseWeakCategoryAsync( string userId, string category);

        Task<List<string>> GetTopWeakCategoriesAsync(string userId);

        Task<List<Scenario>> GetScenariosByAgeGroupAsync(AgeGroupEnum ageGroup, LevelEnum level);

        Task<Scenario?> GetScenarioByIdAsync(int scenarioId);

        Task<int> GetCompletedCountForLevelAsync(string userId, LevelEnum level);

        Task<int> GetUsedHintsCountForLevelAsync(string userId, LevelEnum level);
    }
}
