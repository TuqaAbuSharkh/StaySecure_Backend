using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Models
{
    public enum GenderEnum
    {
        Male = 1,
        Female = 2
    }

    public enum LevelEnum
    {
        Beginner=0,
        Intermediate=1,
        Advanced=2
    }


    public class ApplicationUser :IdentityUser
    {

        public int? Age { get; set; }
        public int TotalScore { get; set; } = 0;

        public string FullName { get; set; }
        public string City { get; set; }

        public GenderEnum? Gender { get; set; } = (GenderEnum?)1;

        public LevelEnum Level { get; set; } = 0;

        public string? CodeResetPassword { get; set; }
        public DateTime? PasswordResetCodeExpiry { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
