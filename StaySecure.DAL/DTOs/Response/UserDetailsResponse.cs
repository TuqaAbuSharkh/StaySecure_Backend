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
        public List<ApplicationUserTranslations> Translations { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public string EmailConfirmed { get; set; }

        public bool IsBlocked { get; set; }
        public List<string> Roles { get; set; }


    }
}
