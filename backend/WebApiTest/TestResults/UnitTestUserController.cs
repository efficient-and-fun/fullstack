using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi;
using WebApi.Model;
using WebApi.Api.Common;

namespace WebApiTest;

[TestClass]
public class UserControllerTests
{
    private Mock<ILogger<MeetUpController>> _loggerMock = null!;
    private Mock<IConfiguration> _configMock = null!;
    private EfDbContext _context = null!;
    private Mock<IAuthService> _authServiceMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<MeetUpController>>();
        _configMock = new Mock<IConfiguration>();
        _authServiceMock = new Mock<IAuthService>();
        
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EfDbContext(options, _configMock.Object);
    }

    [TestMethod]
    public async Task Register_ReturnsOk_WhenRegistrationSuccessful()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Vorname = "Max",
            Nachname = "Mustermann",
            Username = "maxmuster",
            Email = "max@example.com",
            Password = "Password123",
            Password2 = "Password123",
            IsAGBAccepted = true
        };

        _authServiceMock.Setup(s => s.RegisterAsync(
            request.Email, request.Password, request.Vorname, request.Nachname, request.Username))
            .ReturnsAsync(new AuthResult { Success = true, Token = "fake-jwt-token" });

        var controller = new TestableUserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        // Act
        var result = await controller.Register(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("fake-jwt-token", ((dynamic)okResult.Value).token);
    }

    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenPasswordsDoNotMatch()
    {
        var request = new RegisterRequest
        {
            Vorname = "Max",
            Nachname = "Mustermann",
            Username = "maxmuster",
            Email = "max@example.com",
            Password = "Password123",
            Password2 = "WrongPassword",
            IsAGBAccepted = true
        };

        var controller = new TestableUserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        var result = await controller.Register(request);

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual("Passwords do not match.", ((dynamic)badRequest.Value).message);
    }

    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenAGBNotAccepted()
    {
        var request = new RegisterRequest
        {
            Vorname = "Max",
            Nachname = "Mustermann",
            Username = "maxmuster",
            Email = "max@example.com",
            Password = "Password123",
            Password2 = "Password123",
            IsAGBAccepted = false
        };

        var controller = new TestableUserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        var result = await controller.Register(request);

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual("AGB must be accepted.", ((dynamic)badRequest.Value).message);
    }

    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenAuthServiceFails()
    {
        var request = new RegisterRequest
        {
            Vorname = "Max",
            Nachname = "Mustermann",
            Username = "maxmuster",
            Email = "max@example.com",
            Password = "Password123",
            Password2 = "Password123",
            IsAGBAccepted = true
        };

        _authServiceMock.Setup(s => s.RegisterAsync(
            request.Email, request.Password, request.Vorname, request.Nachname, request.Username))
            .ReturnsAsync(new AuthResult { Success = false, ErrorMessage = "E-Mail bereits registriert." });

        var controller = new TestableUserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);

        var result = await controller.Register(request);

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual("E-Mail bereits registriert.", ((dynamic)badRequest.Value).message);
    }
}