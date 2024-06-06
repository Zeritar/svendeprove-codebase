using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemOverseer_UnitTest.Services
{
    public class PCServiceTests
    {
        private readonly Mock<IPCRepository> _mockPCRepository;
        private readonly IPCService _pcService;

        public PCServiceTests()
        {
            _mockPCRepository = new Mock<IPCRepository>();
            _pcService = new PCService(_mockPCRepository.Object);
        }


        [Fact]
        public async Task GetAll_ShouldReturnAllPCs_WhenPCsExist()
        {
            var pcs = new List<PC> { new PC { Id = 1 }, new PC { Id = 2 } };

            _mockPCRepository
                .Setup(r => r.GetAll())
                .ReturnsAsync(pcs);

            var result = await _pcService.GetAll();

            Assert.NotNull(result);
            Assert.Equal(pcs, result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoPCsExist()
        {
            _mockPCRepository
                .Setup(r => r.GetAll())
                .ReturnsAsync(new List<PC>());

            var result = await _pcService.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllForUser_ShouldReturnAllPCsForUser_WhenPCsExist()
        {
            var username = "user";
            var pcs = new List<PC> { new PC { Id = 1 } };

            _mockPCRepository
                .Setup(r => r.GetAllForUser(It.IsAny<string>()))
                .ReturnsAsync(pcs);

            var result = await _pcService.GetAllForUser(username);

            Assert.NotNull(result);
            Assert.Equal(pcs, result);
        }

        [Fact]
        public async Task GetAllForUser_ShouldReturnEmptyList_WhenNoPCsExistForUser()
        {
            var username = "user";

            _mockPCRepository
                .Setup(r => r.GetAllForUser(It.IsAny<string>()))
                .ReturnsAsync(new List<PC>());

            var result = await _pcService.GetAllForUser(username);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetIDs_ShouldReturnAllPCIDs_WhenPCsExist()
        {
            var pcIDs = new List<int> { 1, 2 };

            _mockPCRepository
                .Setup(r => r.GetIDs())
                .ReturnsAsync(pcIDs);

            var result = await _pcService.GetIDs();

            Assert.NotNull(result);
            Assert.Equal(pcIDs, result);
        }

        [Fact]
        public async Task GetIDs_ShouldReturnEmptyList_WhenNoPCsExist()
        {
            _mockPCRepository
                .Setup(r => r.GetIDs())
                .ReturnsAsync(new List<int>());

            var result = await _pcService.GetIDs();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetIDsForUser_ShouldReturnAllPCIDsForUser_WhenPCsExist()
        {
            var userId = "user";
            var pcIDs = new List<int> { 1, 2 };

            _mockPCRepository
                .Setup(r => r.GetIDsForUser(It.IsAny<string>()))
                .ReturnsAsync(pcIDs);

            var result = await _pcService.GetIDsForUser(userId);

            Assert.NotNull(result);
            Assert.Equal(pcIDs, result);
        }

        [Fact]
        public async Task GetIDsForUser_ShouldReturnEmptyList_WhenNoPCsExistForUser()
        {
            var userId = "user";

            _mockPCRepository
                .Setup(r => r.GetIDsForUser(It.IsAny<string>()))
                .ReturnsAsync(new List<int>());

            var result = await _pcService.GetIDsForUser(userId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddPC_ShouldReturnPC_WhenPCIsAddedSuccessfully()
        {
            var pc = new PC { Id = 1 };

            _mockPCRepository
                .Setup(r => r.AddPC(It.IsAny<PC>()))
                .ReturnsAsync(pc);

            var result = await _pcService.AddPC(pc);

            Assert.NotNull(result);
            Assert.Equal(pc, result);
        }

        [Fact]
        public async Task AddPC_ShouldReturnNull_WhenAddingPCFails()
        {
            var pc = new PC { Id = 1 };

            _mockPCRepository
                .Setup(r => r.AddPC(It.IsAny<PC>()))
                .ReturnsAsync(() => null);

            var result = await _pcService.AddPC(pc);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeletePC_ShouldReturnDeletedPC_WhenPCIsDeletedSuccessfully()
        {
            var pc = new PC { Id = 1 };

            _mockPCRepository
                .Setup(r => r.DeletePC(It.IsAny<int>()))
                .ReturnsAsync(pc);

            var result = await _pcService.DeletePC(pc.Id);

            Assert.NotNull(result);
            Assert.Equal(pc, result);
        }

        [Fact]
        public async Task DeletePC_ShouldReturnNull_WhenDeletingPCFails()
        {
            var pcId = 1;

            _mockPCRepository
                .Setup(r => r.DeletePC(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var result = await _pcService.DeletePC(pcId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPC_ShouldReturnPC_WhenPCExists()
        {
            var pc = new PC { Id = 1 };

            _mockPCRepository
                .Setup(r => r.GetPC(It.IsAny<int>()))
                .ReturnsAsync(pc);

            var result = await _pcService.GetPC(pc.Id);

            Assert.NotNull(result);
            Assert.Equal(pc, result);
        }

        [Fact]
        public async Task GetPC_ShouldReturnNull_WhenPCDoesNotExist()
        {
            var pcId = 1;

            _mockPCRepository
                .Setup(r => r.GetPC(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var result = await _pcService.GetPC(pcId);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdatePC_ShouldReturnUpdatedPC_WhenUpdateIsSuccessful()
        {
            var pc = new PC { Id = 1 };

            _mockPCRepository
                .Setup(r => r.UpdatePC(It.IsAny<PC>()))
                .ReturnsAsync(pc);

            var result = await _pcService.UpdatePC(pc);

            Assert.NotNull(result);
            Assert.Equal(pc, result);
        }

        [Fact]
        public async Task UpdatePC_ShouldReturnNull_WhenUpdateFails()
        {
            var pc = new PC { Id = 1 };

            _mockPCRepository
                .Setup(r => r.UpdatePC(It.IsAny<PC>()))
                .ReturnsAsync(() => null);

            var result = await _pcService.UpdatePC(pc);

            Assert.Null(result);
        }
    }
}
