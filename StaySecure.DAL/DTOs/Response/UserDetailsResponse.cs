using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{

    public class UserDetailsResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public AgeGroupEnum AgeGroup { get; set; }
        public int TotalScore { get; set; } = 0;

        public string City { get; set; }

        public GenderEnum? Gender { get; set; } = (GenderEnum?)1;

        public LevelEnum Level { get; set; } = 0;

        public bool EmailConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }
        public bool IsBlocked { get; set; }
        public List<string> Roles { get; set; }

    

    }
}
