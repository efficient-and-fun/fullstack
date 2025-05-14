using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
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
    private Mock<IUserService> _userServiceMock = null!;
    private UserController _controller = null!;


    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<UserController>>();
        _configMock = new Mock<IConfiguration>();
        _authServiceMock = new Mock<IAuthService>();
        _userServiceMock = new Mock<IUserService>();

        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object, _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

        var result = await controller.Login(request);

        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public async Task TestValidate_ReturnsOk_WhenTokenIsValid()
    {
        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer validToken";
        controller.ControllerContext.HttpContext = httpContext;

        var result = controller.Validate();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value;
        Assert.IsNotNull(response);
        Assert.AreEqual("Bearer validToken", (string)response);
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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);
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

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);
        var result = await controller.Login(request);

        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);


        var response = unauthorizedResult.Value as ErrorResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("Invalid credentials", response.Message);
    }

    [TestMethod]
    public async Task GetUsers_ReturnsOkWithUsers_WhenUsersExist()
    {
        // Arrange
        _context.Users.AddRange(new List<User>
        {
            new User
            {
                UserId = 1,
                UserName = "Alice",
                Email = "alice@example.com",
                ProfilePicturePath = "/img/alice.jpg",
                UserPassword = "Password123" // Add a dummy password
            },
            new User
            {
                UserId = 2,
                UserName = "Bob",
                Email = "bob@example.com",
                ProfilePicturePath = "/img/bob.jpg",
                UserPassword = "Password123" // Add a dummy password
            }
        });
        await _context.SaveChangesAsync();

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var users = okResult.Value as List<UserDto>;
        Assert.IsNotNull(users);
        Assert.AreEqual(2, users.Count);
    }

    [TestMethod]
    public async Task GetUsers_ReturnsOkWithEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var users = okResult.Value as List<UserDto>;
        Assert.IsNotNull(users);
        Assert.AreEqual(0, users.Count);
    }

    [TestMethod]
    public async Task GetUsers_MapsUserEntityToDtoCorrectly()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            UserName = "Alice",
            Email = "alice@example.com",
            ProfilePicturePath = "/img/alice.jpg",
            UserPassword = "Password123" // Add a dummy password
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var users = okResult.Value as List<UserDto>;
        Assert.IsNotNull(users);
        var dto = users.First();
        Assert.AreEqual(user.UserId, dto.UserId);
        Assert.AreEqual(user.UserName, dto.UserName);
        Assert.AreEqual(user.Email, dto.Email);
        Assert.AreEqual(user.ProfilePicturePath, dto.ProfilePicturePath);
    }

    [TestMethod]
    public async Task GetFriends_ReturnsOkWithFriends_WhenFriendsExist()
    {
        // Arrange
        var userId = 1;
        var friend = new User
        {
            UserId = 2,
            UserName = "Bob",
            Email = "bob@example.com",
            ProfilePicturePath = "/img/bob.jpg",
            UserPassword = "Password123"
        };
        _context.Users.Add(friend);
        _context.FriendConnection.Add(new FriendConnection
        {
            UserId = userId,
            FriendId = friend.UserId,
            HasAcceptedFriendRequest = true,
            Friend = friend
        });
        await _context.SaveChangesAsync();

        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

        // Act
        var result = await controller.GetFriends();

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var friends = okResult.Value as List<UserDto>;
        Assert.IsNotNull(friends);
        Assert.AreEqual(1, friends.Count);
        Assert.AreEqual(friend.UserId, friends[0].UserId);
        Assert.AreEqual(friend.UserName, friends[0].UserName);
        Assert.AreEqual(friend.Email, friends[0].Email);
        Assert.AreEqual(friend.ProfilePicturePath, friends[0].ProfilePicturePath);
    }

    [TestMethod]
    public async Task GetFriends_ReturnsOkWithEmptyList_WhenNoFriendsExist()
    {
        // Arrange
        var userId = 1;

        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

        // Act
        var result = await controller.GetFriends();

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var friends = okResult.Value as List<UserDto>;
        Assert.IsNotNull(friends);
        Assert.AreEqual(0, friends.Count);
    }

    [TestMethod]
    public async Task GetFriends_ReturnsUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns((int?)null);

        var controller = new UserController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object,
            _userServiceMock.Object);

        // Act
        var result = await controller.GetFriends();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public async Task TestAddFriend_ReturnsOk_WhenFriendIsAdded()
    {
        var userId = 1;
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);
        _userServiceMock.Setup(s => s.AddFriend(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new UserResult { Success = true }));
        
        var result = await _controller.AddFriend("Test");
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var userResult = (result as OkObjectResult)?.Value as UserResult;
        Assert.IsNotNull(userResult);
        Assert.IsTrue(userResult.Success);
    }

    [TestMethod]
    public async Task TestAddFriend_ReturnsUnauthorized_WhenTokenIsNotPresent()
    {
        var result = await _controller.AddFriend("Test");
        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
    }
    
    [TestMethod]
    public async Task TestAddFriend_ReturnsBadRequest_WhenUserDoesNotExist()
    {
        var userId = 1;
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);
        _userServiceMock.Setup(s => s.AddFriend(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new UserResult { Success = false, ErrorMessage = "User not found" }));
        
        var result = await _controller.AddFriend("NotExistingUser");
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var userResult = (UserResult)((BadRequestObjectResult)result).Value!;
        Assert.IsNotNull(userResult);
        Assert.IsFalse(userResult.Success);
        Assert.AreEqual(userResult.ErrorMessage, "User not found");
    }

    [TestMethod]
    public async Task TestAddFriend_ReturnsBadRequest_WhenFriendDoesNotExist()
    {
        var userId = 1;
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);
        _userServiceMock.Setup(s => s.AddFriend(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new UserResult { Success = false, ErrorMessage = "User not found" }));
        
        var result = await _controller.AddFriend("NotExistingUser");
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var userResult = (UserResult)((BadRequestObjectResult)result).Value!;
        Assert.IsNotNull(userResult);
        Assert.IsFalse(userResult.Success);
        Assert.AreEqual(userResult.ErrorMessage, "User not found");
    }

    [TestMethod]
    public async Task TestAddFriend_ReturnsBadRequest_WhenFriendIsTheUserItself()
    {
        var userId = 1;
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);
        _userServiceMock.Setup(s => s.AddFriend(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new UserResult { Success = false, ErrorMessage = "Find real friends" }));
        
        var result = await _controller.AddFriend("Alice");
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var userResult = (UserResult)((BadRequestObjectResult)result).Value!;
        Assert.IsNotNull(userResult);
        Assert.IsFalse(userResult.Success);
        Assert.AreEqual(userResult.ErrorMessage, "Find real friends");
    }

    [TestMethod]
    public async Task TestAddFriend_ReturnsBadRequest_WhenConnectionAlreadyExists()
    {
        var userId = 1;
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);
        _userServiceMock.Setup(s => s.AddFriend(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new UserResult { Success = false, ErrorMessage = "Connection already exists"}));
        
        var result = await _controller.AddFriend("Bob");
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var userResult = (UserResult)((BadRequestObjectResult)result).Value!;
        Assert.IsFalse(userResult.Success);
        Assert.AreEqual("Connection already exists", userResult.ErrorMessage);
    }
    
    [TestMethod]
    public async Task TestRemoveFriend_ReturnsOk_WhenFriendIsRemoved()
    {
        var userId = 1;
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);
        _userServiceMock.Setup(s => s.RemoveFriend(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new UserResult { Success = true }));
        
        // TODO: define case what should happen if HasAcceptedFiendRequest = true;
        
        var result = await _controller.RemoveFriend("Test");
        
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var userResult = (result as OkObjectResult)?.Value as UserResult;
        Assert.IsNotNull(userResult);
        Assert.IsTrue(userResult.Success);
    }
    
    [TestMethod]
    public async Task TestRemoveFriend_ReturnsUnauthorized_WhenTokenIsNotPresent()
    {
        var result = await _controller.RemoveFriend("Test");
        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public async Task TestRemoveFriend_ReturnsBadRequest_WhenUserDoesNotExist()
    {
        var userId = 1;
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);
        _userServiceMock.Setup(s => s.RemoveFriend(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new UserResult { Success = false, ErrorMessage = "User not found" }));
        
        var result = await _controller.RemoveFriend("NotExistingUser");
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var userResult = (result as BadRequestObjectResult)?.Value as UserResult;
        Assert.IsNotNull(userResult);
        Assert.IsFalse(userResult.Success);
        Assert.AreEqual(userResult.ErrorMessage, "User not found");
    }

    [TestMethod]
    public async Task TestRemoveFriend_ReturnsBadRequest_WhenFriendIsTheUserItself()
    {
        var userId = 1;
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);
        _userServiceMock.Setup(s => s.RemoveFriend(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new UserResult { Success = false, ErrorMessage = "User cannot have themselves as friend" }));
        
        var result = await _controller.RemoveFriend("Alice");
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var userResult = (result as BadRequestObjectResult)?.Value as UserResult;
        Assert.IsNotNull(userResult);
        Assert.IsFalse(userResult.Success);
        Assert.AreEqual(userResult.ErrorMessage, "User cannot have themselves as friend");
    }

    [TestMethod]
    public async Task TestRemoveFriend_ReturnsBadRequest_WhenConnectionDoesNotExist()
    {
        var userId = 1;
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);
        _userServiceMock.Setup(s => s.RemoveFriend(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new UserResult { Success = false, ErrorMessage = "Connection does not exist"}));
        
        var result = await _controller.RemoveFriend("Bob");
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var userResult = (result as BadRequestObjectResult)?.Value as UserResult;
        Assert.IsNotNull(userResult);
        Assert.IsFalse(userResult.Success);
        Assert.AreEqual("Connection does not exist", userResult.ErrorMessage);
    }

    [TestMethod]
    public async Task TestRemoveFriend_ReturnsBadRequest_WhenFriendDoesNotExist()
    {
        var userId = 1;
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(userId);
        _userServiceMock.Setup(s => s.RemoveFriend(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new UserResult { Success = false, ErrorMessage = "Friend not found" }));
        
        var result = await _controller.RemoveFriend("NotExistingUser");
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var userResult = (result as BadRequestObjectResult)?.Value as UserResult;
        Assert.IsNotNull(userResult);
        Assert.IsFalse(userResult.Success);
        Assert.AreEqual(userResult.ErrorMessage, "Friend not found");
    }
    
    // TODO: Reuse this when tests for the UserService are added.
    // private async Task SetupDataForFriendConnection()
    // {
    //     try
    //     {
    //         _context.Users.AddRange(new List<User>
    //         {
    //             new User
    //             {
    //                 UserId = 1,
    //                 UserName = "Alice",
    //                 Email = "alice@example.com",
    //                 ProfilePicturePath = "/img/alice.jpg",
    //                 UserPassword = "Password123" // Add a dummy password
    //             },
    //             new User
    //             {
    //                 UserId = 2,
    //                 UserName = "Bob",
    //                 Email = "bob@example.com",
    //                 ProfilePicturePath = "/img/bob.jpg",
    //                 UserPassword = "Password123" // Add a dummy password
    //             },
    //             new User
    //             {
    //                 UserId = 3,
    //                 UserName = "Test",
    //                 Email = "test@example.com",
    //                 ProfilePicturePath = "/img/test.jpg",
    //                 UserPassword = "Password123" // Add a dummy password
    //             }
    //         });
    //
    //         _context.FriendConnection.AddRange(new List<FriendConnection>
    //         {
    //             new FriendConnection()
    //             {
    //                 UserId = 1,
    //                 FriendId = 2,
    //                 HasAcceptedFriendRequest = true,
    //             },
    //             new FriendConnection()
    //             {
    //                 UserId = 2,
    //                 FriendId = 1,
    //                 HasAcceptedFriendRequest = false,
    //             }
    //         });
    //
    //         await _context.SaveChangesAsync();
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         throw;
    //     }
    // }
}