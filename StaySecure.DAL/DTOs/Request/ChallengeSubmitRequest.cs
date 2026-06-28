using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Request
{
    public class ChallengeSubmitRequest
    {
        public int ChallengeId { get; set; }

        public int SelectedOptionId { get; set; }
    }
}
