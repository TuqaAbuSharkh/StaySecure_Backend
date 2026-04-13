using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
           : base(options)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        public IHttpContextAccessor HttpContextAccessor { get; }

        public DbSet<LoginLog> LoginLogs { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }

        public DbSet<ScenarioTranslation> ScenarioTranslations { get; set; }

        public DbSet<ScenarioOption> ScenarioOptions { get; set; }

        public DbSet<ScenarioOptionTranslation> ScenarioOptionTranslations { get; set; }
        public DbSet<UserScenario> UserScenarios { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");


            builder.Entity<Scenario>()
       .HasMany(s => s.Options)
       .WithOne(o => o.Scenario)
       .HasForeignKey(o => o.ScenarioId);

            builder.Entity<Scenario>()
                .HasMany(s => s.Translations)
                .WithOne(t => t.Scenario)
                .HasForeignKey(t => t.ScenarioId);

            builder.Entity<ScenarioOption>()
                .HasMany(o => o.Translations)
                .WithOne(t => t.ScenarioOption)
                .HasForeignKey(t => t.ScenarioOptionId);

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (HttpContextAccessor.HttpContext != null)
            {
                var entres = ChangeTracker.Entries<BaseModel>();
                var currentUserId = HttpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                foreach (var entry in entres)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property(x => x.CreatedBy).CurrentValue = currentUserId;
                        entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;

                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Property(x => x.UpdatedBy).CurrentValue = currentUserId;
                        entry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
        public override int SaveChanges()
        {

            if (HttpContextAccessor.HttpContext != null)
            {
                var entres = ChangeTracker.Entries<BaseModel>();
                var currentUserId = HttpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                foreach (var entry in entres)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property(x => x.CreatedBy).CurrentValue = currentUserId;
                        entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;

                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Property(x => x.UpdatedBy).CurrentValue = currentUserId;
                        entry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
                    }
                }
            }



            return base.SaveChanges();
        }


    }
}
