using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Response
{
    public class LoginResponse :BaseRespose
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool RequiresTwoFactor { get; set; } 
        public string UserId { get; set; }
    }
}
