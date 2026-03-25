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
            TypeAdapterConfig<TranslationDto, ApplicationUserTranslations>
            .NewConfig().Ignore(dest=>dest.ApplicationUser).TwoWays();

            TypeAdapterConfig<ApplicationUser, UserDetailsResponse>.NewConfig()
                .Map(dest => dest.Translations, source => source.Translations);

            TypeAdapterConfig<ApplicationUser, UserListResponse>.NewConfig()
                .Map(dest => dest.UserName, source => source.Translations.Where(t => t.Language == MapContext.Current.Parameters["lang"].ToString())
                 .Select(t => t.FullName).FirstOrDefault());
        }
    }
}
