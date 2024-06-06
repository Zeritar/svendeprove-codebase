using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemOverseer_UnitTest.Repositories
{
    public class PCRepositoryTests
    {
        private readonly PCContext _pcContext;
        private readonly UserContext _userContext;
        private readonly PCRepository _pcRepository;

        public PCRepositoryTests()
        {
            DbContextOptions<PCContext> _pcOptions = new DbContextOptionsBuilder<PCContext>()
                .UseInMemoryDatabase(databaseName: "PCRepositoryDatabase")
                .Options;

            DbContextOptions<UserContext> _userOptions = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "UserRepositoryDatabase")
                .Options;

            _pcContext = new PCContext(_pcOptions);
            _userContext = new UserContext(_userOptions);

            _pcRepository = new PCRepository(_pcContext, _userContext);
        }


        [Fact]
        public async Task GetAll_ShouldReturnAllPCs_WhenPCsExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var pcs = new List<PC> { new PC { Id = 1 }, new PC { Id = 2 } };

            _pcContext.PCs.AddRange(pcs);
            await _pcContext.SaveChangesAsync();

            var result = await _pcRepository.GetAll();

            Assert.NotNull(result);
            Assert.IsType<List<PC>>(result);
            Assert.Equal(pcs.Count, result.Count);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoPCsExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var result = await _pcRepository.GetAll();

            Assert.NotNull(result);
            Assert.IsType<List<PC>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllForUser_ShouldReturnAllPCsForUser_WhenPCsExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();
            await _userContext.Database.EnsureDeletedAsync();

            var user = new ApplicationUser { UserName = "user" };
            await _userContext.Users.AddAsync(user);
            await _userContext.SaveChangesAsync();

            var pcs = new List<PC> { new PC { Id = 1 }, new PC { Id = 2 } };
            _pcContext.PCs.AddRange(pcs);
            await _pcContext.SaveChangesAsync();

            var pcUsers = pcs.Select(pc => new PCUser { UserId = user.Id, PCId = pc.Id }).ToList();
            _pcContext.PCUsers.AddRange(pcUsers);
            await _pcContext.SaveChangesAsync();

            var result = await _pcRepository.GetAllForUser(user.UserName);

            Assert.NotNull(result);
            Assert.IsType<List<PC>>(result);
            Assert.Equal(pcs.Count, result.Count);
        }

        [Fact]
        public async Task GetAllForUser_ShouldReturnEmptyList_WhenNoPCsExistForUser()
        {
            await _pcContext.Database.EnsureDeletedAsync();
            await _userContext.Database.EnsureDeletedAsync();

            var user = new ApplicationUser { UserName = "user" };
            await _userContext.Users.AddAsync(user);
            await _userContext.SaveChangesAsync();

            var result = await _pcRepository.GetAllForUser(user.UserName);

            Assert.NotNull(result);
            Assert.IsType<List<PC>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetIDs_ShouldReturnAllPCIDs_WhenPCsExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var pcs = new List<PC> { new PC { Id = 1 }, new PC { Id = 2 } };
            _pcContext.PCs.AddRange(pcs);
            await _pcContext.SaveChangesAsync();

            var result = await _pcRepository.GetIDs();

            Assert.NotNull(result);
            Assert.IsType<List<int>>(result);
            Assert.Equal(pcs.Select(pc => pc.Id), result);
        }

        [Fact]
        public async Task GetIDs_ShouldReturnEmptyList_WhenNoPCsExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var result = await _pcRepository.GetIDs();

            Assert.NotNull(result);
            Assert.IsType<List<int>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetIDsForUser_ShouldReturnAllPCIDsForUser_WhenPCsExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();
            await _userContext.Database.EnsureDeletedAsync();

            var user = new ApplicationUser { UserName = "user" };
            await _userContext.Users.AddAsync(user);
            await _userContext.SaveChangesAsync();

            var pcs = new List<PC> { new PC { Id = 1 }, new PC { Id = 2 } };
            _pcContext.PCs.AddRange(pcs);
            await _pcContext.SaveChangesAsync();

            var pcUsers = pcs.Select(pc => new PCUser { UserId = user.Id, PCId = pc.Id }).ToList();
            _pcContext.PCUsers.AddRange(pcUsers);
            await _pcContext.SaveChangesAsync();

            var result = await _pcRepository.GetIDsForUser(user.UserName);

            Assert.NotNull(result);
            Assert.IsType<List<int>>(result);
            Assert.True(result.All(id => pcs.Any(pc => pc.Id == id)));
        }

        [Fact]
        public async Task GetIDsForUser_ShouldReturnEmptyList_WhenNoPCsExistForUser()
        {
            await _pcContext.Database.EnsureDeletedAsync();
            await _userContext.Database.EnsureDeletedAsync();

            var user = new ApplicationUser { UserName = "user" };
            await _userContext.Users.AddAsync(user);
            await _userContext.SaveChangesAsync();

            var result = await _pcRepository.GetIDsForUser(user.UserName);

            Assert.NotNull(result);
            Assert.IsType<List<int>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddPC_ShouldReturnAddedPC_WhenAddIsSuccessful()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var pc = new PC { Id = 1 };

            var result = await _pcRepository.AddPC(pc);

            Assert.NotNull(result);
            Assert.Equal(pc, result);
        }

        [Fact]
        public async Task AddPC_ShouldReturnNull_WhenAddFails()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var pc = new PC { Id = 1 };

            _pcContext.PCs.Add(pc);
            await _pcContext.SaveChangesAsync();

            var result = await _pcRepository.AddPC(pc);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPC_ShouldReturnPC_WhenPCExists()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var pc = new PC { Id = 1 };
            _pcContext.PCs.Add(pc);
            await _pcContext.SaveChangesAsync();

            var result = await _pcRepository.GetPC(pc.Id);

            Assert.NotNull(result);
            Assert.Equal(pc, result);
        }

        [Fact]
        public async Task GetPC_ShouldReturnNull_WhenPCDoesNotExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();

            var result = await _pcRepository.GetPC(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdatePC_ShouldReturnUpdatedPC_WhenUpdateIsSuccessful()
        {
            await _pcContext.Database.EnsureDeletedAsync();
        
            var pc = new PC { Id = 1, Name = "OldName" };
            _pcContext.PCs.Add(pc);
            await _pcContext.SaveChangesAsync();
        
            pc.Name = "NewName";
            var result = await _pcRepository.UpdatePC(pc);
        
            Assert.NotNull(result);
            Assert.Equal("NewName", result.Name);
        }
        
        [Fact]
        public async Task UpdatePC_ShouldReturnNull_WhenPCDoesNotExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();
        
            var pc = new PC { Id = 1, Name = "OldName" };
        
            var result = await _pcRepository.UpdatePC(pc);
        
            Assert.Null(result);
        }

        [Fact]
        public async Task DeletePC_ShouldReturnDeletedPC_WhenDeleteIsSuccessful()
        {
            await _pcContext.Database.EnsureDeletedAsync();
        
            var pc = new PC { Id = 1 };
            _pcContext.PCs.Add(pc);
            await _pcContext.SaveChangesAsync();
        
            var result = await _pcRepository.DeletePC(pc.Id);
        
            Assert.NotNull(result);
            Assert.Equal(pc, result);
        }
        
        [Fact]
        public async Task DeletePC_ShouldReturnNull_WhenPCDoesNotExist()
        {
            await _pcContext.Database.EnsureDeletedAsync();
        
            var result = await _pcRepository.DeletePC(1);
        
            Assert.Null(result);
        }
    }
}
