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
    public class ChallengeRepository : IChallengeRepository
    {
        private readonly ApplicationDbContext _context;

        public ChallengeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetUserAsync(string userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
