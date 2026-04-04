using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Request
{
    public class TwoFactorRequest
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }
}
