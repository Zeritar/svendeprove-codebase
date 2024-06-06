namespace SystemOverseer_UnitTest.Controllers
{
    public class PCUserControllerTests
    {
        private readonly PCUserController _pcUserController;
        private Mock<IPCUserService> _pcUserServiceMock = new();
        // placeholder token for user with username "admin" and role "admin" for controller authorization
        private readonly string _token = "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJqdGkiOiJhNmMxYzEyMy03MThiLTRmOWYtYjFmOC02ODYwOThkZWFhY2MiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOlsidXNlciIsImFkbWluIl0sImV4cCI6MTcxNjkwMTQ2NywiaXNzIjoiU3lzdGVtT3ZlcnNlZXJBUEkiLCJhdWQiOiJTeXN0ZW1PdmVyc2VlckNsaWVudCJ9.RdD-Lvd6SEn4FVUl5bNhqhB3aMV91Ei32jg3NUAxyYI";

        public PCUserControllerTests()
        {
            _pcUserController = new PCUserController(_pcUserServiceMock.Object);
            var contextMock = new Mock<HttpContext>();
            contextMock.SetupGet(x => x.Request.Headers["Authorization"]).Returns(_token);
            _pcUserController.ControllerContext.HttpContext = contextMock.Object;
        }

        [Fact]
        public async Task GetAllForUser_Should200()
        {
            var pcUsers = new List<PCUser> { new PCUser { Id = 1, PCId = 1, UserId = "adminGUID" } };

            _pcUserServiceMock
            .Setup(s => s.GetAllForUser(It.IsAny<string>()))
            .ReturnsAsync(pcUsers);

            var result = (IStatusCodeActionResult)await _pcUserController.GetAllForUser("admin");

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task GetAllForPC_ShouldReturnPCUsers_WhenPCExists()
        {
            var pcUsers = new List<PCUser> { new PCUser { Id = 1, PCId = 1, UserId = "adminGUID" } };

            _pcUserServiceMock
            .Setup(s => s.GetAllForPC(It.IsAny<int>()))
            .ReturnsAsync(pcUsers);

            var result = (IStatusCodeActionResult)await _pcUserController.GetAllForPC(1);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task AddPCToUser_ShouldReturnPCUser_WhenSuccessful()
        {
            var pcUser = new PCUser { Id = 1, PCId = 1, UserId = "adminGUID" };
            var pcUserDTO = new PCUserDTO() { PCId = pcUser.PCId, Username = "admin" };

            _pcUserServiceMock
            .Setup(s => s.AddPCToUser(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(pcUser);

            var result = (IStatusCodeActionResult)await _pcUserController.AddPCToUser(pcUserDTO);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task AddPCToUser_ShouldReturnBadRequest_WhenAddFails()
        {
            var pcUserDTO = new PCUserDTO() { PCId = 1, Username = "admin" };

            _pcUserServiceMock
            .Setup(s => s.AddPCToUser(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync((PCUser?)null);

            var result = (IStatusCodeActionResult)await _pcUserController.AddPCToUser(pcUserDTO);

            Assert.Equal(400, result.StatusCode);
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unable to add PC to user", ((BadRequestObjectResult)result).Value);
        }

        [Fact]
        public async Task DeletePCUser_ShouldReturnPCUser_WhenSuccessful()
        {
            var pcUser = new PCUser { Id = 1, PCId = 1, UserId = "adminGUID" };

            _pcUserServiceMock
            .Setup(s => s.DeletePCUser(It.IsAny<int>()))
            .ReturnsAsync(pcUser);

            var result = (IStatusCodeActionResult)await _pcUserController.DeletePCUser(1);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task DeletePCUser_ShouldReturnNotFound_WhenDeleteFails()
        {
            _pcUserServiceMock
            .Setup(s => s.DeletePCUser(It.IsAny<int>()))
            .ReturnsAsync((PCUser?)null);

            var result = (IStatusCodeActionResult)await _pcUserController.DeletePCUser(1);

            Assert.Equal(404, result.StatusCode);
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("PCUser not found", ((NotFoundObjectResult)result).Value);
        }
    }
}