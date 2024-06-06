using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SystemOverseer_API.Controllers;

namespace SystemOverseer_UnitTest.Controllers
{
    public class PCAdminControllerTests
    {
        private readonly PCAdminController _pcAdminController;
        private Mock<IPCService> _pcServiceMock = new();

        public PCAdminControllerTests()
        {
            _pcAdminController = new PCAdminController(_pcServiceMock.Object);
            _pcAdminController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async void GetAll_ShouldReturn200()
        {
            List<PC> pcs = new();

            _pcServiceMock
                .Setup(x => x.GetAll())
            .ReturnsAsync(pcs);

            var result = (IStatusCodeActionResult)await _pcAdminController.GetAll();

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void Get_ShouldReturn200()
        {
            int id = 1;
            PC pc = new() { Id = id };

            _pcServiceMock
                .Setup(x => x.GetPC(id))
            .ReturnsAsync(pc);

            var result = (IStatusCodeActionResult)await _pcAdminController.Get(id);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void Create_ShouldReturn200()
        {
            PCDTO pcdto = new()
            {
                Name = "PC1",
                IsNetworked = true,
                IpAddress = 1337,
                MacAddress = new uint[1] { 0x00 },
                Pins = new uint[1] { 0x00 },
                IsOnline = false
            };

            PC pc = new()
            {
                Name = pcdto.Name,
                IsNetworked = pcdto.IsNetworked,
                IpAddress = pcdto.IpAddress,
                MacAddress = pcdto.MacAddress.Select(e => (byte)e).ToArray(),
                Pins = pcdto.Pins.Select(e => (byte)e).ToArray(),
                IsOnline = pcdto.IsOnline
            };

            _pcServiceMock
                .Setup(x => x.AddPC(pc))
            .ReturnsAsync(pc);

            var result = (IStatusCodeActionResult)await _pcAdminController.Create(pcdto);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void Update_ShouldReturn200()
        {
            PCDTO pcdto = new()
            {
                Name = "PC1",
                IsNetworked = true,
                IpAddress = 1337,
                MacAddress = new uint[1] { 0x00 },
                Pins = new uint[1] { 0x00 },
                IsOnline = false
            };

            PC pc = new()
            {
                Name = pcdto.Name,
                IsNetworked = pcdto.IsNetworked,
                IpAddress = pcdto.IpAddress,
                MacAddress = pcdto.MacAddress.Select(e => (byte)e).ToArray(),
                Pins = pcdto.Pins.Select(e => (byte)e).ToArray(),
                IsOnline = pcdto.IsOnline
            };

            _pcServiceMock
                .Setup(x => x.UpdatePC(pc))
            .ReturnsAsync(pc);

            var result = (IStatusCodeActionResult)await _pcAdminController.Update(pcdto);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void Delete_ShouldReturn200()
        {
            int id = 1;
            PC pc = new() { Id = id };

            _pcServiceMock
                .Setup(x => x.DeletePC(id))
            .ReturnsAsync(pc);

            var result = (IStatusCodeActionResult)await _pcAdminController.Delete(id);

            Assert.Equal(200, result.StatusCode);
        }
    }
}
