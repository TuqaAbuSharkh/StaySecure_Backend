using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.DTOs.Request
{
    public class RegisterRequest
    {

        public string Email { get; set; }

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string City { get; set; }

        public string Password { get; set; }
        public int Age { get; set; }
        public GenderEnum Gender { get; set; }


    }
}
    