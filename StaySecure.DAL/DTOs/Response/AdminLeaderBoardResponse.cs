using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class AdminLeaderboardResponse
    {
        public List<LeaderBoardResponse> Children { get; set; }

        public List<LeaderBoardResponse> Teens { get; set; }

        public List<LeaderBoardResponse> Adults { get; set; }
    }

}
