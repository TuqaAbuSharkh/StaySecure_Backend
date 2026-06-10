using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class ScenarioListResponse
    {
        public int Id { get; set; }

        public AgeGroupEnum AgeGroup { get; set; }

        public LevelEnum Level { get; set; }

        public int Score { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }
    }
}
