using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public UserSeedData(UserManager<ApplicationUser> roleManager,IConfiguration configuration)
        {
            _userManager = roleManager;
            _configuration = configuration;
        }
        public async Task DataSeed()
        {
            if (!await _userManager.Users.AnyAsync())
            {
              
                var user1 = new ApplicationUser
                {
                    UserName = "Tuqa",
                    FullName = "Tuqa Abu Sharkh",
                    City = "Hebron",
                    Email = "abusharktuqa@gmail.com",
                    EmailConfirmed = true,
                    Gender= GenderEnum.Female
                };

                await _userManager.CreateAsync(user1, _configuration["SeedUsers:User1"]!);

                await _userManager.AddToRoleAsync(user1, "Admin");

                var user2 = new ApplicationUser
                {
                    UserName = "Rand",
                    Email = "randhaymouni@gmail.com",
                    FullName = "Rand Haymouni",
                    City = "Hebron",
                    EmailConfirmed = true,
                    Gender = GenderEnum.Female
                };

                await _userManager.CreateAsync(user2, _configuration["SeedUsers:User2"]!);

                await _userManager.AddToRoleAsync(user2, "Admin");


                var user3 = new ApplicationUser
                {
                    UserName = "Majd",
                    FullName = "Majd Tamimi",
                    City = "Hebron",
                    Email = "majd2004tamimi@gmail.com",
                    EmailConfirmed = true,
                    Gender = GenderEnum.Female
                };

                await _userManager.CreateAsync(user3, _configuration["SeedUsers:User3"]!);

                await _userManager.AddToRoleAsync(user3, "Admin");
            }
        }

    }

}
