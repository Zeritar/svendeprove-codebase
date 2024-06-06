using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemOverseer_UnitTest.Services
{
    public class PCUserServiceTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IPCUserRepository> _mockPCUserRepository;
        private readonly Mock<IPCService> _mockPCService;
        private readonly PCUserService _pcUserService;

        public PCUserServiceTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockPCUserRepository = new Mock<IPCUserRepository>();
            _mockPCService = new Mock<IPCService>();
            _pcUserService = new PCUserService(_mockUserService.Object, _mockPCUserRepository.Object, _mockPCService.Object);
        }

        [Fact]
        public async Task GetAllForUser_ShouldReturnPCUsers_WhenUserExists()
        {
            var user = new ApplicationUser { Id = "adminGUID", UserName = "admin" };
            var pcUsers = new List<PCUser> { new PCUser { Id = 1, PCId = 1, UserId = user.Id } };

            _mockUserService
            .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

            _mockPCUserRepository
            .Setup(x => x.GetAllForUser(It.IsAny<string>()))
            .ReturnsAsync(pcUsers);

            var result = await _pcUserService.GetAllForUser(user.UserName);

            Assert.NotNull(result);
            Assert.Equal(pcUsers, result);
        }

        [Fact]
        public async Task GetAllForUser_ShouldReturnEmptyList_WhenUserDoesNotExist()
        {
            _mockUserService
            .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);

            var result = await _pcUserService.GetAllForUser("nonexistentUser");

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllForPC_ShouldReturnPCUsers_WhenPCExists()
        {
            var pcUsers = new List<PCUser> { new PCUser { Id = 1, PCId = 1, UserId = "adminGUID" } };

            _mockPCUserRepository
            .Setup(x => x.GetAllForPC(It.IsAny<int>()))
            .ReturnsAsync(pcUsers);

            var result = await _pcUserService.GetAllForPC(1);

            Assert.NotNull(result);
            Assert.Equal(pcUsers, result);
        }

        [Fact]
        public async Task GetAllForPC_ShouldReturnEmptyList_WhenNoPCUsersExist()
        {
            _mockPCUserRepository
            .Setup(x => x.GetAllForPC(It.IsAny<int>()))
            .ReturnsAsync(new List<PCUser>());

            var result = await _pcUserService.GetAllForPC(1);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddPCToUser_ShouldReturnPCUser_WhenSuccessful()
        {
            var user = new ApplicationUser { Id = "adminGUID", UserName = "admin" };
            var pcUser = new PCUser { Id = 1, PCId = 1, UserId = user.Id };

            _mockUserService.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockPCUserRepository.Setup(x => x.AddPCUser(It.IsAny<PCUser>())).ReturnsAsync(pcUser);

            var result = await _pcUserService.AddPCToUser(pcUser.PCId, user.UserName);

            Assert.NotNull(result);
            Assert.Equal(pcUser, result);
        }

        [Fact]
        public async Task AddPCToUser_ShouldReturnNull_WhenUserDoesNotExist()
        {
            _mockUserService
            .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);

            var result = await _pcUserService.AddPCToUser(1, "nonexistentUser");

            Assert.Null(result);
        }

        [Fact]
        public async Task DeletePCUser_ShouldReturnPCUser_WhenSuccessful()
        {
            var pcUser = new PCUser { Id = 1, PCId = 1, UserId = "adminGUID" };

            _mockPCUserRepository
            .Setup(x => x.DeletePCUser(It.IsAny<int>()))
            .ReturnsAsync(pcUser);

            var result = await _pcUserService.DeletePCUser(pcUser.Id);

            Assert.NotNull(result);
            Assert.Equal(pcUser, result);
        }

        [Fact]
        public async Task DeletePCUser_ShouldReturnNull_WhenPCUserDoesNotExist()
        {
            _mockPCUserRepository
            .Setup(x => x.DeletePCUser(It.IsAny<int>()))
            .ReturnsAsync(() => null);

            var result = await _pcUserService.DeletePCUser(1);

            Assert.Null(result);
        }
    }
}
