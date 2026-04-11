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

        
    }
}
