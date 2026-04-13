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

    }
}
