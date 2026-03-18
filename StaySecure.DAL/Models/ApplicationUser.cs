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


    public class ApplicationUser :IdentityUser
    {

       public List<ApplicationUserTranslations> Translations { get; set; }
        public int? Age { get; set; }
        public int TotalScore { get; set; } = 0;

        public GenderEnum? Gender { get; set; } = (GenderEnum?)1;


        public string? CodeResetPassword { get; set; }
        public DateTime? PasswordResetCodeExpiry { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
