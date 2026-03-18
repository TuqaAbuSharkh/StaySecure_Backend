using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Utls
{
    public class UserSeedData : ISeedData
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSeedData(UserManager<ApplicationUser> roleManager)
        {
            _userManager = roleManager;
        }
        public async Task DataSeed()
        {
            if (!await _userManager.Users.AnyAsync())
            {
                var user1 = new ApplicationUser
                {
                    UserName = "Tuqa",
                    Email = "abusharktuqa@gmail.com",
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(user1, "Pass@2005");

                await _userManager.AddToRoleAsync(user1, "Admin");
            }
        }

    }

}
