using StaySecure.DAL.DTOs.Request;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services.IServices
{
    public interface IManageUserService
    {
        Task<List<UserListResponse>> GetUsersAsync();
        Task<UserDetailsResponse> GetUserDetailsAsync(string Id);
        Task<BaseRespose> BlockedUserAsync(string userId);
        Task<BaseRespose> UnBlockedUserAsync(string userId);
        Task<BaseRespose> ChangeUserRoleAsync(ChangeUserRoleRequest request);
        Task<List<LoginLog>> GetLoginLogsAsync(string userId);
        Task<List<LoginLog>> GetAllLoginLogsAsync();


    }
}
