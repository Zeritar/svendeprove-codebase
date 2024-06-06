namespace SystemOverseer_UnitTest.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _authController;
        private Mock<IAuthService> _authServiceMock = new();

        public AuthControllerTests()
        {
            _authController = new AuthController(_authServiceMock.Object);
            _authController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async void Login_ShouldReturnStatusCode200_OnValidLogin()
        {
            TokenModel token = new();
            LoginModel login = new() { Username = "ValidUser", Password = "ValidPassword" };

            _authServiceMock
                .Setup(x => x.Login(login))
                .ReturnsAsync(token);

            var result = (IStatusCodeActionResult)await _authController.Login(login);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void Login_ShouldReturnStatusCode401_OnInvalidLogin()
        {
            TokenModel? token = null;
            LoginModel login = new() { Username = "ValidUser", Password = "InvalidPassword" };

            _authServiceMock
                .Setup(x => x.Login(login))
                .ReturnsAsync(token);

            var result = (IStatusCodeActionResult)await _authController.Login(login);

            Assert.Equal(401, result.StatusCode);
        }

        [Fact]
        public async void Register_ShouldReturnStatusCode200_OnRegistrationSuccess()
        {
            AuthResponse response = new() { Status = "Success" };
            RegisterModel register = new() { Username = "NewUser", Password = "NewPassword", Email = "NewEmail" };

            _authServiceMock
                .Setup(x => x.Register(register))
                .ReturnsAsync(response);

            var result = (IStatusCodeActionResult)await _authController.Register(register);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void Register_ShouldReturnStatusCode400_OnRegistrationError()
        {
            AuthResponse response = new() { Status = "Error" };
            RegisterModel register = new() { Username = "NewUser", Password = "NewPassword", Email = "NewEmail" };

            _authServiceMock
                .Setup(x => x.Register(register))
                .ReturnsAsync(response);

            var result = (IStatusCodeActionResult)await _authController.Register(register);

            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async void RegisterAdmin_ShouldReturnStatusCode200_OnRegistrationSuccess()
        {
            AuthResponse response = new() { Status = "Success" };
            RegisterModel register = new() { Username = "NewUser", Password = "NewPassword", Email = "NewEmail" };

            _authServiceMock
                .Setup(x => x.RegisterAdmin(register))
                .ReturnsAsync(response);

            var result = (IStatusCodeActionResult)await _authController.RegisterAdmin(register);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void RegisterAdmin_ShouldReturnStatusCode400_OnRegistrationError()
        {
            AuthResponse response = new() { Status = "Error" };
            RegisterModel register = new() { Username = "NewUser", Password = "NewPassword", Email = "NewEmail" };

            _authServiceMock
                .Setup(x => x.RegisterAdmin(register))
                .ReturnsAsync(response);

            var result = (IStatusCodeActionResult)await _authController.RegisterAdmin(register);

            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async void ConfirmEmail_ShouldReturnStatusCode200_OnSuccess()
        {
            AuthResponse response = new() { Status = "Success" };
            string username = "ConfirmUser";

            _authServiceMock
                .Setup(x => x.ConfirmEmail(username))
                .ReturnsAsync(response);

            var result = (IStatusCodeActionResult)await _authController.ConfirmEmail(username);

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void ConfirmEmail_ShouldReturnStatusCode400_OnError()
        {
            AuthResponse response = new() { Status = "Error" };
            string username = "ConfirmUser";

            _authServiceMock
                .Setup(x => x.ConfirmEmail(username))
                .ReturnsAsync(response);

            var result = (IStatusCodeActionResult)await _authController.ConfirmEmail(username);

            Assert.Equal(400, result.StatusCode);
        }
    }
}
