using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SystemOverseer_UnitTest.Services
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IUserService> _mockUserService;

        public AuthServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            var inMemorySettings = new Dictionary<string, string?> {
                {"JWT:Secret", "xunitsecret1234567890xunitsecret"},
                {"JWT:ValidIssuer", "xunitissuer"},
                {"JWT:ValidAudience", "xunitaudience"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            _mockUserService = new Mock<IUserService>();
            _authService = new AuthService(configuration, _mockUserService.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnTokenModel_WhenValidLoginModelIsProvided()
        {
            LoginModel loginModel = new LoginModel { Username = "user", Password = "Passw0rd!" };

            _mockUserService
            .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { UserName = loginModel.Username });

            _mockUserService
            .Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);

            _mockUserService
            .Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "User" });

            var result = await _authService.Login(loginModel);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Login_ShouldReturnNull_WhenWrongPasswordIsProvided()
        {
            LoginModel loginModel = new LoginModel { Username = "user", Password = "WrongPassw0rd!" };

            _mockUserService
            .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { UserName = loginModel.Username });

            _mockUserService
            .Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);

            var result = await _authService.Login(loginModel);

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_ShouldReturnNull_WhenUserIsNotFound()
        {
            LoginModel loginModel = new LoginModel { Username = "invaliduser", Password = "Passw0rd!" };

            _mockUserService
            .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);

            var result = await _authService.Login(loginModel);

            Assert.Null(result);
        }

        [Fact]
        public async Task Register_ShouldReturnSuccess_WhenUserIsCreatedSuccessfully()
        {
            RegisterModel registerModel = new RegisterModel { Username = "newuser", Password = "Passw0rd!", Email = "newuser@test.com" };

            _mockUserService
            .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);

            _mockUserService
            .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

            var result = await _authService.Register(registerModel);

            Assert.Equal("Success", result.Status);
            Assert.Equal("User created successfully!", result.Message);
        }

        [Fact]
        public async Task Register_ShouldReturnError_WhenUserAlreadyExists()
        {
            RegisterModel registerModel = new RegisterModel { Username = "existinguser", Password = "Passw0rd!", Email = "existinguser@test.com" };

            _mockUserService
            .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { UserName = registerModel.Username });

            var result = await _authService.Register(registerModel);

            Assert.Equal("Error", result.Status);
            Assert.Equal("User already exists!", result.Message);
        }

        [Fact]
        public async Task RegisterAdmin_ShouldReturnSuccess_WhenAdminIsCreatedSuccessfully()
        {
            RegisterModel registerModel = new RegisterModel { Username = "newadmin", Password = "Passw0rd!", Email = "newadmin@test.com" };

            _mockUserService
            .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);

            _mockUserService
            .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

            _mockUserService
            .Setup(u => u.RoleExistsAsync(UserRoles.Admin))
            .ReturnsAsync(true);

            _mockUserService
            .Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRoles.Admin))
            .ReturnsAsync(IdentityResult.Success);

            var result = await _authService.RegisterAdmin(registerModel);

            Assert.Equal("Success", result.Status);
            Assert.Equal("User created successfully!", result.Message);
        }

        [Fact]
        public async Task RegisterAdmin_ShouldReturnError_WhenUserAlreadyExists()
        {
            RegisterModel registerModel = new RegisterModel { Username = "existingadmin", Password = "Passw0rd!", Email = "existingadmin@test.com" };

            _mockUserService
            .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { UserName = registerModel.Username });

            var result = await _authService.RegisterAdmin(registerModel);

            Assert.Equal("Error", result.Status);
            Assert.Equal("User already exists!", result.Message);
        }
    }
}
