using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class ScenarioOverviewResponse
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }
        public LevelEnum Level { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsUnlocked { get; set; }
    }
}
