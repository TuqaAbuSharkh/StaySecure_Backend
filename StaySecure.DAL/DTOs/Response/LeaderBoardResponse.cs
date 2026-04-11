using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class LeaderBoardResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public int TotalScore { get; set; }
        public LevelEnum Level { get; set; }

        public int? Rank { get; set; }

    }
}
