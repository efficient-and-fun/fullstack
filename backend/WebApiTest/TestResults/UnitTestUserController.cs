using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi;
using WebApi.Model;
using WebApi.Api.Common;
using Microsoft.EntityFrameworkCore;

namespace WebApiTest;

[TestClass]
public class UserControllerTests
{
    private Mock<ILogger<UserController>> _loggerMock = null!;
    private Mock<IConfiguration> _configMock = null!;
    private EfDbContext _context = null!;
    private Mock<IAuthService> _authServiceMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<UserController>>();
        _configMock = new Mock<IConfiguration>();
        _authServiceMock = new Mock<IAuthService>();
        
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EfDbContext(options, _configMock.Object);
    }

    [TestMethod]
    public async Task TestRegister_ReturnsOk_WhenRegistrationSuccessful()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "maxmuster",
            Email = "max@example.com",
            Password = "Password123",
            Password2 = "Password123",
            ProfilePicturePath = "testPath",
            IsAGBAccepted = true
        };

        _authServiceMock.Setup(s => s.RegisterAsync(
                request.Email, request.Password, request.Username, request.ProfilePicturePath))
            .ReturnsAsync(new AuthResult { Success = true, Token = "dummy-token" });

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        // Act
        var result = await controller.Register(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as TokenResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("dummy-token", response.Token);
    }
    
    [TestMethod]
    public async Task TestRegister_ReturnsBadRequest_WhenEmailAlreadyRegistered()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "maxmuster",
            Email = "existing@example.com",
            Password = "Password123",
            Password2 = "Password123",
            ProfilePicturePath = "testPath",
            IsAGBAccepted = true
        };

        // Simuliere den Fehlerfall im AuthService
        _authServiceMock.Setup(s => s.RegisterAsync(
                request.Email, request.Password, request.Username, request.ProfilePicturePath))
            .ReturnsAsync(new AuthResult
            {
                Success = false,
                ErrorMessage = "Email already registered."
            });

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        // Act
        var result = await controller.Register(request);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);

        var error = badRequest.Value as ErrorResponse;
        Assert.IsNotNull(error);
        Assert.AreEqual("Email already registered.", error.Message);
    }
    
    [TestMethod]
    public async Task TestRegister_ReturnsBadRequest_WhenUsernameAlreadyRegistered()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "existinguser",
            Email = "newuser@example.com",
            Password = "Password123",
            Password2 = "Password123",
            ProfilePicturePath = "testPath",
            IsAGBAccepted = true
        };

        // Simuliere den Fehlerfall im AuthService
        _authServiceMock.Setup(s => s.RegisterAsync(
                request.Email, request.Password, request.Username, request.ProfilePicturePath))
            .ReturnsAsync(new AuthResult
            {
                Success = false,
                ErrorMessage = "Username already registered."
            });

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        // Act
        var result = await controller.Register(request);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);

        var error = badRequest.Value as ErrorResponse;
        Assert.IsNotNull(error);
        Assert.AreEqual("Username already registered.", error.Message);
    }
    
    [TestMethod]
    public async Task TestRegister_ReturnsBadRequest_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "maxmuster",
            Email = "max@example.com",
            Password = "Password123",
            Password2 = "DifferentPassword123", // Mismatch
            ProfilePicturePath = "testPath",
            IsAGBAccepted = true
        };

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        // Act
        var result = await controller.Register(request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);

        var response = badRequestResult.Value as dynamic;
        Assert.IsNotNull(response);
        Assert.AreEqual("Passwords do not match.", response.Message);
    }

    [TestMethod]
    public async Task TestRegister_ReturnsBadRequest_WhenAGBNotAccepted()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "maxmuster",
            Email = "max@example.com",
            Password = "Password123",
            Password2 = "Password123", // Match
            ProfilePicturePath = "testPath",
            IsAGBAccepted = false // AGB not accepted
        };

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        // Act
        var result = await controller.Register(request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);

        var response = badRequestResult.Value as ErrorResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("AGB must be accepted.", response.Message); // Updated to 'Message'
    }

    [TestMethod]
    public async Task TestRegister_ReturnsBadRequest_WhenAuthServiceFails()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "maxmuster",
            Email = "max@example.com",
            Password = "Password123",
            Password2 = "Password123", // Match
            ProfilePicturePath = "testPath",
            IsAGBAccepted = true
        };

        // Simuliere einen fehlerhaften AuthService
        _authServiceMock.Setup(s => s.RegisterAsync(
                request.Email, request.Password, request.Username, request.ProfilePicturePath))
            .ReturnsAsync(new AuthResult { Success = false, ErrorMessage = "Username already registered." });

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        // Act
        var result = await controller.Register(request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);

        var response = badRequestResult.Value as ErrorResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("Username already registered.", response.Message);
    }
    
    

    [TestMethod]
    public async Task TestValidate_ReturnsUnauthorized_WhenTokenIsNotPresent()
    {
        var request = new LoginRequest
        {
            Email = "max@example.com",
            Password = "Password123"
        };

        _authServiceMock.Setup(s => s.LoginAsync(request.Email, request.Password))
            .ReturnsAsync(new AuthResult { Success = false, ErrorMessage = "Token not present" });

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);
        
        var result = await controller.Login(request);
        
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }
    
    [TestMethod]
    public async Task TestValidate_ReturnsUnauthorized_WhenTokenIsInvalid()
    {
        var request = new LoginRequest
        {
            Email = "max@example.com",
            Password = "InvalidPassword"
        };

        _authServiceMock.Setup(s => s.LoginAsync(request.Email, request.Password))
            .ReturnsAsync(new AuthResult { Success = false, ErrorMessage = "Invalid token" });

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);
        
        var result = await controller.Login(request);
        
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public async Task TestValidate_ReturnsOk_WhenTokenIsValid()
    {
            var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer validToken";
            controller.ControllerContext.HttpContext = httpContext;
            
            var result = controller.Validate();
            
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value;
            Assert.IsNotNull(response);
            Assert.AreEqual("Bearer validToken", (string) response);
        
    }
    [TestMethod]
    public async Task TestRegister_ReturnsOk_WhenProfilePicturePathExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "maxmuster",
            Email = "max@example.com",
            Password = "Password123",
            Password2 = "Password123",
            ProfilePicturePath = "testPath",
            IsAGBAccepted = true
        };

        _authServiceMock.Setup(s => s.RegisterAsync(
                request.Email, request.Password, request.Username, request.ProfilePicturePath))
            .ReturnsAsync(new AuthResult { Success = true, Token = "dummy-token" });

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        // Act
        var result = await controller.Register(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as TokenResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("dummy-token", response.Token);
    }
    [TestMethod]
    public async Task TestRegister_ReturnsOk_WhenProfilePicturePathNotExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "maxmuster",
            Email = "max@example.com",
            Password = "Password123",
            Password2 = "Password123",
            ProfilePicturePath = "",
            IsAGBAccepted = true
        };

        _authServiceMock.Setup(s => s.RegisterAsync(
                request.Email, request.Password, request.Username, request.ProfilePicturePath))
            .ReturnsAsync(new AuthResult { Success = true, Token = "dummy-token" });

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        // Act
        var result = await controller.Register(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as TokenResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("dummy-token", response.Token);
    }

    [TestMethod]
    public async Task TestLogin_ReturnsOk_WhenUserLoginRequestDataIsValid()
    {
        var request = new LoginRequest()
        {
            Email = "max@example.com",
            Password = "Password123",
        };
        
        _authServiceMock.Setup(s => s.LoginAsync(request.Email, request.Password))
            .ReturnsAsync(new AuthResult { Success = true, Token = "dummy-token" });
        
        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);
        
        var result = await controller.Login(request);
        
        var loginResult = result as OkObjectResult;
        Assert.IsNotNull(loginResult);
        Assert.AreEqual(200, loginResult.StatusCode);
        
        var response = loginResult.Value as TokenResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("dummy-token", response.Token);
    }

    [TestMethod]
    public async Task TestLogin_ReturnsUnauthorized_WhenNoUserWithEmailExists()
    {
        var request = new LoginRequest
        {
            Email = "max@example.com",
            Password = "Password123",
        };
        
        _authServiceMock.Setup(s => s.LoginAsync(request.Email, request.Password))
            .ReturnsAsync(new AuthResult { Success = false });
        
        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);
        var result = await controller.Login(request);
        
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
        
        
        var response = unauthorizedResult.Value as ErrorResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("Invalid credentials", response.Message);
    }
    
    [TestMethod]
    public async Task TestLogin_ReturnsUnauthorized_WhenPasswordForExistingUserIsWrong()
    {
        var request = new LoginRequest
        {
            Email = "max@example.com",
            Password = "Password123",
        };
        
        _authServiceMock.Setup(s => s.LoginAsync(request.Email, request.Password))
            .ReturnsAsync(new AuthResult { Success = false });
        
        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);
        var result = await controller.Login(request);
        
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
        
        
        var response = unauthorizedResult.Value as ErrorResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("Invalid credentials", response.Message);
    }
}