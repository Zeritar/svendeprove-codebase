using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemOverseer_UnitTest.Services
{
    public class UserServiceTests
    {
        private readonly IdentityUserService _identityUserService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;

        public UserServiceTests()
        {
            // Identity UserManager is dependency injected per scope, so we need a Scope Factory to mock it.
            var _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            var serviceProvider = new Mock<IServiceProvider>();
            var serviceScope = new Mock<IServiceScope>();

            _mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(),
            null, null, null, null);

            serviceProvider
                .Setup(x => x.GetService(typeof(UserManager<ApplicationUser>)))
                .Returns(_mockUserManager.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(RoleManager<IdentityRole>)))
                .Returns(_mockRoleManager.Object);

            serviceScope
                .Setup(x => x.ServiceProvider)
                .Returns(serviceProvider.Object);

            _mockServiceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            _identityUserService = new IdentityUserService(_mockServiceScopeFactory.Object);
        }

        [Fact]
        public async Task FindByNameAsync_ShouldReturnUser_WhenUserExists()
        {
            var username = "user";
            _mockUserManager
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser { UserName = username });

            var result = await _identityUserService.FindByNameAsync(username);

            Assert.NotNull(result);
            Assert.Equal(username, result.UserName);
        }

        [Fact]
        public async Task FindByNameAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var username = "no-user";
            _mockUserManager
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var result = await _identityUserService.FindByNameAsync(username);

            Assert.Null(result);
        }

        [Fact]
        public async Task CheckPasswordAsync_ShouldReturnTrue_WhenPasswordIsValid()
        {
            var user = new ApplicationUser { UserName = "user" };
            var password = "Passw0rd!";
            _mockUserManager
                .Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await _identityUserService.CheckPasswordAsync(user, password);

            Assert.True(result);
        }

        [Fact]
        public async Task CheckPasswordAsync_ShouldReturnFalse_WhenPasswordIsInvalid()
        {
            var user = new ApplicationUser { UserName = "user" };
            var password = "P@ssw0rd!";
            _mockUserManager
                .Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _identityUserService.CheckPasswordAsync(user, password);

            Assert.False(result);
        }

        [Fact]
        public async Task GetRolesAsync_ShouldReturnListOfRoles_WhenUserHasRoles()
        {
            var user = new ApplicationUser { UserName = "user" };
            _mockUserManager
                .Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            var result = await _identityUserService.GetRolesAsync(user);

            Assert.NotNull(result);
            Assert.Equal("User", result.First());
        }

        [Fact]
        public async Task GetRolesAsync_ShouldReturnEmptyList_WhenUserHasNoRoles()
        {
            var user = new ApplicationUser { UserName = "user" };
            _mockUserManager
                .Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string>());

            var result = await _identityUserService.GetRolesAsync(user);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenUserIsCreatedSuccessfully()
        {
            var user = new ApplicationUser { UserName = "newuser", Email = "newuser@test.com" };
            var password = "Passw0rd!";

            _mockUserManager
                .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _identityUserService.CreateAsync(user, password);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFailure_WhenUserCreationFails()
        {
            var user = new ApplicationUser { UserName = "newuser", Email = "newuser@test.com" };
            var password = "Passw0rd!";

            _mockUserManager
                .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            var result = await _identityUserService.CreateAsync(user, password);

            Assert.NotNull(result);
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task RoleExistsAsync_ShouldReturnTrue_WhenRoleExists()
        {
            var roleName = "Admin";

            _mockRoleManager
                .Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await _identityUserService.RoleExistsAsync(roleName);

            Assert.True(result);
        }

        [Fact]
        public async Task RoleExistsAsync_ShouldReturnFalse_WhenRoleDoesNotExist()
        {
            var roleName = "NonExistentRole";

            _mockRoleManager
                .Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _identityUserService.RoleExistsAsync(roleName);

            Assert.False(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenRoleIsCreatedSuccessfully()
        {
            var role = new IdentityRole { Name = "NewRole" };

            _mockRoleManager
                .Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _identityUserService.CreateAsync(role);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFailure_WhenRoleCreationFails()
        {
            var role = new IdentityRole { Name = "NewRole" };

            _mockRoleManager
                .Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed());

            var result = await _identityUserService.CreateAsync(role);

            Assert.NotNull(result);
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task AddToRoleAsync_ShouldReturnSuccess_WhenUserIsAddedToRoleSuccessfully()
        {
            var user = new ApplicationUser { UserName = "user" };
            var role = "Admin";

            _mockUserManager
                .Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _identityUserService.AddToRoleAsync(user, role);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task AddToRoleAsync_ShouldReturnFailure_WhenAddingUserToRoleFails()
        {
            var user = new ApplicationUser { UserName = "user" };
            var role = "Admin";

            _mockUserManager
                .Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            var result = await _identityUserService.AddToRoleAsync(user, role);

            Assert.NotNull(result);
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task ConfirmEmailAsync_ShouldReturnSuccess_WhenEmailIsConfirmedSuccessfully()
        {
            var username = "user";

            var user = new ApplicationUser { UserName = username, Email = "existinguser@test.com" };

            _mockUserManager
                .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _identityUserService.ConfirmEmailAsync(username);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task ConfirmEmailAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            var username = "no-user";

            _mockUserManager
                .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var result = await _identityUserService.ConfirmEmailAsync(username);

            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal("user", result.Errors.First().Code);
            Assert.Equal("User not found", result.Errors.First().Description);
        }
    }
}
