using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SystemOverseer_API.Auth;

namespace SystemOverseer_API.Models
{
    public class UserContext : IdentityDbContext<ApplicationUser>
    {

        public static string userGuid = "14318bc8-b64f-4629-9238-27079bd7abf2";
        public static string adminGuid = "e484c160-d4f6-4220-a144-26e6201ceb72";
        public static string userRoleGuid = "14318bc8-b64f-4629-9238-27079bd7abf3";
        public static string adminRoleGuid = "e484c160-d4f6-4220-a144-26e6201ceb73";
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ApplicationUser user = new ApplicationUser
            {
                Id = userGuid,
                UserName = "user",
                NormalizedUserName = "USER",
                Email = "user@so.com",
                EmailConfirmed = true,
                LockoutEnabled = false
            };

            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Passw0rd!");

            ApplicationUser admin = new ApplicationUser
            {
                Id = adminGuid,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@so.com",
                EmailConfirmed = true,
                LockoutEnabled = false
            };

            IdentityRole userRole = new IdentityRole
            {
                Id = userRoleGuid,
                Name = Auth.UserRoles.User,
                NormalizedName = Auth.UserRoles.User.ToUpper()
            };

            IdentityRole adminRole = new IdentityRole
            {
                Id = adminRoleGuid,
                Name = Auth.UserRoles.Admin,
                NormalizedName = Auth.UserRoles.Admin.ToUpper()
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, "Passw0rd!");
            modelBuilder.Entity<ApplicationUser>().HasData(new List<ApplicationUser>() { user, admin });
            modelBuilder.Entity<IdentityRole>().HasData(new List<IdentityRole>() { userRole, adminRole });
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new List<IdentityUserRole<string>>()
            {
                new IdentityUserRole<string> { UserId = userGuid, RoleId = userRoleGuid },
                new IdentityUserRole<string> { UserId = adminGuid, RoleId = userRoleGuid },
                new IdentityUserRole<string> { UserId = adminGuid, RoleId = adminRoleGuid }
            });
        }
    }
}
