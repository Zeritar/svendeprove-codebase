using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemOverseer_UnitTest.Repositories
{
    public class PCUserRepositoryTests
    {
        private readonly PCContext _pcContext;
        private readonly PCUserRepository _pcUserRepository;

        public PCUserRepositoryTests()
        {
            DbContextOptions<PCContext> _pcOptions = new DbContextOptionsBuilder<PCContext>()
                .UseInMemoryDatabase(databaseName: "PCUserRepositoryDatabase")
                .Options;

            _pcContext = new PCContext(_pcOptions);
            _pcUserRepository = new PCUserRepository(_pcContext);
        }

        [Fact]
        public async Task GetAllForUser_ShouldReturnPCUsers_WhenUserExists()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var pcUsers = new List<PCUser> { new PCUser { Id = 1, PCId = 1, UserId = "adminGUID" } };

            _pcContext.PCUsers.AddRange(pcUsers);
            await _pcContext.SaveChangesAsync();

            var result = await _pcUserRepository.GetAllForUser("adminGUID");

            Assert.NotNull(result);
            Assert.IsType<List<PCUser>>(result);
            Assert.Equal(pcUsers.Count, result.Count);
        }

        [Fact]
        public async Task GetAllForUser_ShouldReturnEmptyList_WhenUserDoesNotExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var result = await _pcUserRepository.GetAllForUser("adminGUID");

            Assert.NotNull(result);
            Assert.IsType<List<PCUser>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllForPC_ShouldReturnPCUsers_WhenPCExists()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var pcUsers = new List<PCUser> { new PCUser { Id = 1, PCId = 1, UserId = "adminGUID" } };

            _pcContext.PCUsers.AddRange(pcUsers);
            await _pcContext.SaveChangesAsync();

            var result = await _pcUserRepository.GetAllForPC(1);

            Assert.NotNull(result);
            Assert.IsType<List<PCUser>>(result);
            Assert.Equal(pcUsers.Count, result.Count);
        }

        [Fact]
        public async Task GetAllForPC_ShouldReturnEmptyList_WhenPCDoesNotExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var result = await _pcUserRepository.GetAllForPC(1);

            Assert.NotNull(result);
            Assert.IsType<List<PCUser>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddPCUser_ShouldAddPCUser_WhenPCUserIsValid()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var pcUser = new PCUser { Id = 1, PCId = 1, UserId = "adminGUID" };

            var result = await _pcUserRepository.AddPCUser(pcUser);

            Assert.NotNull(result);
            Assert.Equal(pcUser, result);
        }

        [Fact]
        public async Task DeletePCUser_ShouldDeletePCUser_WhenPCUserExists()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var pcUser = new PCUser { Id = 1, PCId = 1, UserId = "adminGUID" };

            _pcContext.PCUsers.Add(pcUser);
            await _pcContext.SaveChangesAsync();

            var result = await _pcUserRepository.DeletePCUser(1);

            Assert.NotNull(result);
            Assert.Equal(pcUser, result);
        }

        [Fact]
        public async Task DeletePCUser_ShouldReturnNull_WhenPCUserDoesNotExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var result = await _pcUserRepository.DeletePCUser(1);

            Assert.Null(result);
        }
    }
}
