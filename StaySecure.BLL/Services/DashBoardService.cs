using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.Data;
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
    public class DashBoardService : IDashBoardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IScenarioRepository _scenarioRepository;

        public DashBoardService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IScenarioRepository scenarioRepository)
        {
            _userManager = userManager;
            _context = context;
            _scenarioRepository = scenarioRepository;
        }
        public async Task<UserProgressResponse> GetUserProgressAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception("UserId is null");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");


            var userScenarios = await _scenarioRepository.GetUserScenariosAsync(userId);

            userScenarios = userScenarios ?? new List<UserScenario>();

            var completed = userScenarios.Count;

            var correct = userScenarios.Count(s => s.IsCorrect);

            double successRate = completed == 0
                ? 0
                : (double)correct / completed * 100;

            return new UserProgressResponse
            {
                TotalScore = user.TotalScore,
                Level = user.Level,
                CompletedScenarios = completed,
                CorrectAnswers = correct,
                SuccessRate = Math.Round(successRate, 2)
            };
        }



        public async Task<List<LeaderBoardResponse>> GetLeaderboardAsync(string userId)
        {
            var currentUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (currentUser == null)
                throw new Exception("User not found");

            var users = await _userManager.Users
                .Where(u => u.AgeGroup == currentUser.AgeGroup)
                .OrderByDescending(u => u.TotalScore)
                .ToListAsync();

            var result = new List<LeaderBoardResponse>();

            int rank = 0;
            int previousScore = -1;

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];

                if (user.TotalScore != previousScore)
                {
                    rank = i + 1;
                    previousScore = user.TotalScore;
                }

                result.Add(new LeaderBoardResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    TotalScore = user.TotalScore,
                    Level = user.Level,
                    Age = user.Age,
                    Rank = rank
                });
            }

            return result;
        }

    }
}
