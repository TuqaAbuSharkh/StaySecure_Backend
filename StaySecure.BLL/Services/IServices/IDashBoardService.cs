using StaySecure.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services.IServices
{
    public interface IDashBoardService
    {
        Task<UserProgressResponse> GetUserProgressAsync(string userId);
        Task<List<LeaderBoardResponse>> GetLeaderboardAsync(string userId);


    }
}
