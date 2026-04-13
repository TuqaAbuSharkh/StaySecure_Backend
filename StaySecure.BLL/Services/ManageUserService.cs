using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.Data;
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
    public class ManageUserService : IManageUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IScenarioRepository _scenarioRepository;

        public ManageUserService(UserManager<ApplicationUser> userManager,ApplicationDbContext context, IScenarioRepository scenarioRepository)
        {
            _userManager = userManager;
            _context = context;
            _scenarioRepository = scenarioRepository;
        }
        public async Task<List<UserListResponse>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = users.Adapt<List<UserListResponse>>();

            for (int i = 0; i < users.Count; i++)
            {
                var roles = await _userManager.GetRolesAsync(users[i]);
                result[i].Roles = roles.ToList();

                result[i].IsBlocked =
                    users[i].LockoutEnd != null &&
                    users[i].LockoutEnd > DateTimeOffset.UtcNow;
            }

            return result;
        }
        public async Task<UserDetailsResponse> GetUserDetailsAsync(string Id)
        {
            var user = await _userManager.Users
              .FirstOrDefaultAsync(u => u.Id == Id);

            if (user == null)
            {
                return null;
            }
            var result = user.Adapt<UserDetailsResponse>();
            var roles = await _userManager.GetRolesAsync(user);
            result.Roles = roles.ToList();
            
            return result;
        }

        public async Task<BaseRespose> BlockedUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "User already blocked"
                };
            }

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            return new BaseRespose
            {
                Success = true,
                Message = "User blocked successfully"
            };
        }

        public async Task<BaseRespose> UnBlockedUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            if (!await _userManager.IsLockedOutAsync(user))
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "User is not blocked"
                };
            }

            await _userManager.SetLockoutEndDateAsync(user, null);

            return new BaseRespose
            {
                Success = true,
                Message = "User unblocked successfully"
            };
        }

        public async Task<BaseRespose> ChangeUserRoleAsync(ChangeUserRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "Failed!"
                };
            }
            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "Failed!"
                };
            }

            var addResult = await _userManager.AddToRoleAsync(user, request.Role);

            if (!addResult.Succeeded)
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "Failed !"
                };
            }

            return new BaseRespose
            {
                Success = true,
                Message = "Role updated successfully"
            };
        }
        

        public async Task<List<LoginLog>> GetLoginLogsAsync(string userId)
        {
            return await _context.LoginLogs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.AttemptTime)
                .Select(x => new LoginLog
                {
                    Email = x.Email,
                    AttemptTime = x.AttemptTime,
                    Success = x.Success,
                    FailureReason = x.FailureReason,
                    IpAddress = x.IpAddress
                })
                .ToListAsync();
        }

        public async Task<List<LoginLog>> GetAllLoginLogsAsync()
        {
            return await _context.LoginLogs
                .OrderByDescending(x => x.AttemptTime)
                .Select(x => new LoginLog
                {
                    Email = x.Email,
                    AttemptTime = x.AttemptTime,
                    Success = x.Success,
                    FailureReason = x.FailureReason,
                    IpAddress = x.IpAddress
                })
                .ToListAsync();
        }


        public async Task<List<LeaderBoardResponse>> GetLeaderboardAsync(string userId)
        {
            var currentUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (currentUser == null)
                throw new Exception("User not found");

            var users = await _userManager.Users
                .Where(u => u.Id != userId &&
                            u.Level == currentUser.Level &&
                            u.AgeGroup == currentUser.AgeGroup)
                .OrderByDescending(u => u.TotalScore)
                .Take(10)
                .ToListAsync();

            var result = users.Select((u, index) => new LeaderBoardResponse
            {
                Id = u.Id,
                UserName = u.UserName,
                TotalScore = u.TotalScore,
                Level = u.Level,
                Age = u.Age,
                Rank = index + 1
            }).ToList();

            return result;
        }


        public async Task<BaseRespose> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            user.FullName = request.FullName;
            user.Age = request.Age;
            user.City = request.City;
            user.Gender = request.Gender;

            user.AgeGroup = (AgeGroupEnum)CalculateAgeGroup(user.Age);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new BaseRespose
                {
                    Success = false,
                    Message = "Update failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new BaseRespose
            {
                Success = true,
                Message = "Profile updated successfully"
            };
        }

        private int CalculateAgeGroup(int age)
        {
            if (age <= 11) return 1;
            if (age <= 18) return 2;
            return 3;
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



    }
}
