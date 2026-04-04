using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Models
{
    public class LoginLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public string Email { get; set; }  
        public DateTime AttemptTime { get; set; }
        public bool Success { get; set; }
        public string IpAddress { get; set; }
        public string? FailureReason { get; set; }  

    }
}
