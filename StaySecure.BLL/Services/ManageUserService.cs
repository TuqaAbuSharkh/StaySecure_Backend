using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.Data;
using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
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


        public ManageUserService(UserManager<ApplicationUser> userManager,ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
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
            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Role);
            return new BaseRespose
            {
                Success = true,
                Message = "role Updated "
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



    }


}
