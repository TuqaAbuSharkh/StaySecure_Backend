using StaySecure.BLL.Services;
using StaySecure.BLL.Services.IServices;
using Microsoft.AspNetCore.Identity.UI.Services;
using StaySecure.DAL.Utls;

namespace StaySecure.PL
{
    public class AppConfigration
    {
        public static void Config(IServiceCollection Services)
        {

            Services.AddTransient<IEmailSender, EmailSender>();
            Services.AddScoped<IAuthinticationService, AuthenticationService>();
            Services.AddScoped<ITokenService, TokenService>();
            Services.AddScoped<IManageUserService, ManageUserService>();

            Services.AddScoped<ISeedData, RoleSeedData>();
            Services.AddScoped<ISeedData, UserSeedData>();


        }
    }
}
