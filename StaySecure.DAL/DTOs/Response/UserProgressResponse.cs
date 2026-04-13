using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class UserProgressResponse
    {
        public int TotalScore { get; set; }

        public LevelEnum Level { get; set; }

        public int CompletedScenarios { get; set; }

        public int CorrectAnswers { get; set; }

        public double SuccessRate { get; set; }
    }

}
