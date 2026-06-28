using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class ChallengeSubmitResponse
    {
        public bool IsCorrect { get; set; }

        public int CorrectAnswerId { get; set; }

        public string Message { get; set; }

        public string Feedback { get; set; }

        public int EarnedScore { get; set; }
    }
}
