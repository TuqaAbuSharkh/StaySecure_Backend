
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StaySecure.BLL;
using StaySecure.BLL.MapsterConfigration;
using StaySecure.DAL.Data;
using StaySecure.DAL.Models;
using StaySecure.DAL.Utls;
using StaySecure.PL;
using Stripe;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace StaySecure
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var MyAllowSpecificOrigins = "_myAllowOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:5174")
                                                        .AllowAnyHeader()
                                                        .AllowAnyMethod()
                                                        .AllowCredentials();
                                  });
            });
            // Add services to the container.

            builder.Services.AddControllers();


            //for rate limiter
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("GlobalPolicy", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromSeconds(2);
                    opt.QueueLimit = 0;
                });
            });


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;

                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 4;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                options.SignIn.RequireConfirmedEmail = true;
            })
              .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:secretKey"]!))
        };
    });


            builder.Services.AddLocalization(options => options.ResourcesPath = "");
            const string defaultCulture = "en";
            var supportedCultures = new[]
            {
              new CultureInfo(defaultCulture),
              new CultureInfo("ar")
            };
            builder.Services.Configure<RequestLocalizationOptions>(options => {
                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider
                {
                    QueryStringKey ="lang"
                });
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            AppConfigration.Config(builder.Services);
            MapsterConfig.MapsterConfigRegister();

            var app = builder.Build();

            app.UseRouting();
            app.UseRateLimiter();

            app.UseCors(MyAllowSpecificOrigins); 

            app.UseAuthentication();   
            app.UseAuthorization();

            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseHttpsRedirection();

            app.MapControllers();


            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seeders = services.GetServices<ISeedData>();

                foreach (var seeder in seeders)
                {
                    await seeder.DataSeed();
                }
            }

          

            app.Run();
        }
    }
}
