using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Data;
using SystemOverseer_API.Auth;
using SystemOverseer_API.Models;
using SystemOverseer_API.Repositories;

namespace SystemOverseer_API.Services
{

    public interface IUserService
    {
        Task<List<string>> GetAllUsers();
        Task<ApplicationUser?> FindByNameAsync(string username);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<IdentityResult> CreateAsync(IdentityRole role);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> ConfirmEmailAsync(string username);
    }

    public class HardcodedUserService : IUserService
    {
        private static readonly List<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser { UserName = "admin", PasswordHash = "admin" },
            new ApplicationUser { UserName = "user", PasswordHash = "user" }
        };

        private static readonly Dictionary<string, IList<string>> _roles = new Dictionary<string, IList<string>>
        {
            { UserRoles.Admin.ToLower(), new List<string> { "admin" } },
            { UserRoles.User.ToLower(), new List<string> { "user", "admin" } }
        };

        public async Task<ApplicationUser?> FindByNameAsync(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }

            username = NormalizeName(username);
            return await Task.FromResult(_users.FirstOrDefault(u => u.UserName == username));
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (user == null)
            {
                return await Task.FromResult(false);
            }

            if (password == null)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(password == "Passw0rd!");
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return await Task.FromResult(_roles.Where(e => e.Value.Contains(user.UserName)).Select(e => e.Key).ToList());
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "user", Description = "User is null" });
            }

            if (password == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "password", Description = "Password is null" });
            }

            ApplicationUser newUser = new ApplicationUser
            {
                UserName = user.UserName,
                PasswordHash = user.UserName
            };

            _users.Add(newUser);
            await AddToRoleAsync(user, UserRoles.User);

            return await Task.FromResult(IdentityResult.Success);
        }

        private string NormalizeName(string name)
        {
            return name.ToLower();
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            if (roleName == null)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(_roles.ContainsKey(NormalizeName(roleName)));
        }

        public Task<IdentityResult> CreateAsync(IdentityRole role)
        {
            if (role == null)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "role", Description = "Role is null" }));
            }

            _roles.Add(NormalizeName(role.Name), new List<string>() { NormalizeName(role.Name) });

            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "user", Description = "User is null" });
            }

            string normalizedRole = NormalizeName(role);
            if (_roles.TryGetValue(NormalizeName(user.UserName), out IList<string>? uroles) != false)
            {
                if (uroles.Contains(normalizedRole))
                    return IdentityResult.Failed(new IdentityError { Code = "user", Description = $"User already has role {role}" });
            }

            _roles[normalizedRole].Add(NormalizeName(user.UserName));
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string username)
        {
            return IdentityResult.Success;
        }

        public Task<List<string>> GetAllUsers()
        {
            return Task.FromResult(_users.Select(e => e.UserName).ToList());
        }
    }

    public class IdentityUserService : IUserService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IdentityUserService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<List<string>> GetAllUsers()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await userManager.Users.Select(e => e.UserName).ToListAsync();
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string username)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await userManager.FindByNameAsync(username);
            }
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await userManager.CheckPasswordAsync(user, password);
            }
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await userManager.GetRolesAsync(user);
            }
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await userManager.CreateAsync(user, password);
            }
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                return await roleManager.RoleExistsAsync(roleName);
            }
        }

        public async Task<IdentityResult> CreateAsync(IdentityRole role)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                return await roleManager.CreateAsync(role);
            }
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await userManager.AddToRoleAsync(user, role);
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string username)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.FindByNameAsync(username);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Code = "user", Description = "User not found" });
                user.EmailConfirmed = true;
                return await userManager.UpdateAsync(user);
            }
        }
    }
}
