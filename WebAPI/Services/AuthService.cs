using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SystemOverseer_API.Auth;
using SystemOverseer_API.Models;

namespace SystemOverseer_API.Services
{
    public interface IAuthService
    {
        Task<TokenModel?> Login(LoginModel model);
        Task<AuthResponse> Register(RegisterModel model);
        Task<AuthResponse> RegisterAdmin(RegisterModel model);
        Task<AuthResponse> ConfirmEmail (string username);
    }
    public class AuthService : IAuthService
    {
        
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthService(IConfiguration configuration, IUserService userService)
        {
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<AuthResponse> ConfirmEmail(string username)
        {
            var result = await _userService.ConfirmEmailAsync(username);
            if (!result.Succeeded)
                return new AuthResponse { Status = "Error", Message = $"Email confirmation failed." };
            else
                return new AuthResponse { Status = "Success", Message = "Email confirmed." };
        }

        public async Task<TokenModel?> Login(LoginModel model)
        {
            var user = await _userService.FindByNameAsync(model.Username);
            if (user != null && await _userService.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userService.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return new TokenModel() {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                };
            }

            return null;
        }

        public async Task<AuthResponse> Register(RegisterModel model)
        {
            var userExists = await _userService.FindByNameAsync(model.Username);
            if (userExists != null)
                return new AuthResponse { Status = "Error", Message = "User already exists!" };

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userService.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return new AuthResponse { Status = "Error", Message = "User creation failed! Please check user details and try again." };

            return new AuthResponse { Status = "Success", Message = "User created successfully!" };
        }

        public async Task<AuthResponse> RegisterAdmin(RegisterModel model)
        {
            var userExists = await _userService.FindByNameAsync(model.Username);
            if (userExists != null)
                return new AuthResponse { Status = "Error", Message = "User already exists!" };

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await _userService.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return new AuthResponse { Status = "Error", Message = "User creation failed! Please check user details and try again." };

            if (!await _userService.RoleExistsAsync(UserRoles.Admin))
                await _userService.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _userService.RoleExistsAsync(UserRoles.User))
                await _userService.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _userService.RoleExistsAsync(UserRoles.Admin))
            {
                await _userService.AddToRoleAsync(user, UserRoles.Admin);
            }

            return new AuthResponse { Status = "Success", Message = "User created successfully!" };
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
