namespace SystemOverseer_UnitTest.Controllers
{
    public class PCControllerTests
    {
        private readonly PCController _pcController;
        private Mock<IPCService> _pcServiceMock = new();
        // placeholder token for user with username "admin" and role "admin" for controller authorization
        private readonly string _token = "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJqdGkiOiJhNmMxYzEyMy03MThiLTRmOWYtYjFmOC02ODYwOThkZWFhY2MiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOlsidXNlciIsImFkbWluIl0sImV4cCI6MTcxNjkwMTQ2NywiaXNzIjoiU3lzdGVtT3ZlcnNlZXJBUEkiLCJhdWQiOiJTeXN0ZW1PdmVyc2VlckNsaWVudCJ9.RdD-Lvd6SEn4FVUl5bNhqhB3aMV91Ei32jg3NUAxyYI";

        public PCControllerTests()
        {
            _pcController = new PCController(_pcServiceMock.Object);
            var contextMock = new Mock<HttpContext>();
            contextMock.SetupGet(x => x.Request.Headers["Authorization"]).Returns(_token);
            _pcController.ControllerContext.HttpContext = contextMock.Object;
        }

        [Fact]
        public async void GetIDs_ShouldReturn200()
        {
            List<int> pcIds = new();

            _pcServiceMock
            .Setup(x => x.GetIDs())
            .ReturnsAsync(pcIds);

            var result = (IStatusCodeActionResult)await _pcController.GetIDs();

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void GetIDsForUser_ShouldReturn200_WhenValidToken()
        {
            List<int> pcIds = new();
            string username = "admin";

            _pcServiceMock
            .Setup(x => x.GetIDsForUser(username))
            .ReturnsAsync(pcIds);

            var result = (IStatusCodeActionResult)await _pcController.GetIDsForUser();

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void GetAll_ShouldReturn200()
        {
            List<PC> pcs = new();

            _pcServiceMock
            .Setup(x => x.GetAll())
            .ReturnsAsync(pcs);

            var result = (IStatusCodeActionResult)await _pcController.GetAll();

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void GetAllForUser_ShouldReturn200_WhenValidToken()
        {
            List<PC> pcs = new();
            string username = "admin";

            _pcServiceMock
            .Setup(x => x.GetAllForUser(username))
            .ReturnsAsync(pcs);

            var result = (IStatusCodeActionResult)await _pcController.GetAllForUser();

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void Start_ShouldReturn200()
        {
            int id = 1;
            CommandDTO command = new CommandDTO()
            {
                Command = "start",
                Body = new PCRequest()
                {
                    Id = id
                }
            };

            _pcServiceMock
            .Setup(x => x.SendCommand(command))
            .ReturnsAsync(true);

            var result = (IStatusCodeActionResult)await _pcController.Start(id);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void Shutdown_ShouldReturn200()
        {
            int id = 1;
            CommandDTO command = new CommandDTO()
            {
                Command = "shutdown",
                Body = new PCRequest()
                {
                    Id = id
                }
            };

            _pcServiceMock
            .Setup(x => x.SendCommand(command))
            .ReturnsAsync(true);

            var result = (IStatusCodeActionResult)await _pcController.Shutdown(id);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void Heartbeat_ShouldReturn200()
        {
            int id = 1;
            CommandDTO command = new CommandDTO()
            {
                Command = "heartbeat",
                Body = new PCRequest()
                {
                    Id = id
                }
            };

            _pcServiceMock
            .Setup(x => x.SendCommand(command))
            .ReturnsAsync(true);

            var result = (IStatusCodeActionResult)await _pcController.Heartbeat(id);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void Status_ShouldReturn200()
        {
            string[] strings = new string[8];
            List<PC> pcs = new List<PC> {
                new PC { Name = "PC0", IsOnline = false },
                new PC { Name = "PC1", IsOnline = false },
                new PC { Name = "PC2", IsOnline = false },
                new PC { Name = "PC3", IsOnline = false } };

            for (int i = 0; i < 4; i++)
            {

                strings[i * 2] = $"PC{i}";
                strings[i * 2 + 1] = "OFFLINE";
            }

            UpdateDTO command = new UpdateDTO()
            {
                Command = "status",
                Body = strings
            };

            _pcServiceMock.Setup(x => x.GetAll()).ReturnsAsync(pcs);

            _pcServiceMock
            .Setup(x => x.SendCommand(command))
            .ReturnsAsync(true);

            var result = (IStatusCodeActionResult)await _pcController.Status();

            Assert.Equal(200, result.StatusCode);
        }
    }
}
