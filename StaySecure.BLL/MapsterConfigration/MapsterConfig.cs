using Mapster;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.MapsterConfigration
{
    public static class MapsterConfig
    {

        public static void MapsterConfigRegister()
        {
            TypeAdapterConfig<ApplicationUser, UserDetailsResponse>
             .NewConfig()
             .Map(dest => dest.IsBlocked,
                  src => src.LockoutEnd != null && src.LockoutEnd > DateTimeOffset.UtcNow);
        }
    }
}
