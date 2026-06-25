using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Repositories.Interface
{
    public interface IChallengeRepository
    {
        Task<ApplicationUser?> GetUserAsync(string userId);
    }
}
