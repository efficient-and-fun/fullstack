using System.Security.Claims;
using WebApi;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApi.Api.Common;
using WebApi.Model;

namespace WebApiTest;

[TestClass]
public class UnitTestMeetUpController
{
    private MeetUpController _meetUpController;

    private Mock<ILogger<MeetUpController>> _mockLogger;
    private Mock<IConfiguration> _mockConfig;
    private Mock<EfDbContext> _mockContext;
    private Mock<DbContextOptions<EfDbContext>> _mockDbContextOptions;

    private Mock<HttpContext> _mockHttpContext;
    private Mock<HttpResponse> _mockHttpResponse;
    private Mock<IResponseCookies> _mockCookies;

    [TestInitialize]
    public void TestMeetUpController()
    {
        _mockDbContextOptions = new Mock<DbContextOptions<EfDbContext>>();

        _mockLogger = new Mock<ILogger<MeetUpController>>();
        _mockConfig = new Mock<IConfiguration>();

        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        _mockCookies = new Mock<IResponseCookies>();

        _mockHttpResponse = new Mock<HttpResponse>();
        _mockHttpResponse.SetupGet(r => r.Cookies).Returns(_mockCookies.Object);

        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);

        _meetUpController = new MeetUpController(_mockLogger.Object, _mockConfig.Object, context)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = _mockHttpContext.Object,
            }
        };
    }

    [TestMethod]
    public void TestCreateMeetUp_ReturnsBadRequest_WhenUserIdIsZero()
    {
        // Arrange
        var meetUpDto = new MeetUpDetailDto { MeetUpName = "Test MeetUp", DateTimeFrom = DateTime.Now, DateTimeTo = DateTime.Now.AddHours(1), CheckList = "Test", MeetUpLocation = "Location", Description = "Description" };

        // Act
        var result = _meetUpController.CreateMeetUp(0, meetUpDto);
        
        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual("UserId invalid", badRequestResult.Value);
    }
    
    [TestMethod]
    public void TestCreateMeetUp_ReturnsBadRequest_WhenUserIdIsNegative()
    {
        // Arrange
        var meetUpDto = new MeetUpDetailDto { MeetUpName = "Test Meet Up", DateTimeFrom = DateTime.Now, DateTimeTo = DateTime.Now.AddHours(1), CheckList = "Test", MeetUpLocation = "Location", Description = "Description" };

        // Act
        var result = _meetUpController.CreateMeetUp(-1, meetUpDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual("UserId invalid", badRequestResult.Value);
    }
    
    [TestMethod]
    public void TestCreateMeetUp_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange – Use real EF context with InMemory provider
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        // No users are added here to simulate a non-existing user

        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        var newMeetUp = new MeetUpDetailDto()
        {
            MeetUpName = "New Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(2),
            CheckList = "Checklist",
            MeetUpLocation = "Test Location",
            Description = "Test Description"
        };
        var nonExistentUserId = 999; // Assume this user does not exist

        // Act
        var result = controller.CreateMeetUp(nonExistentUserId, newMeetUp);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.IsInstanceOfType(notFoundResult.Value, typeof(string)); // Assuming the value is a string message
        Assert.AreEqual($"User with ID {nonExistentUserId} does not exist.", notFoundResult.Value); // Adjust the message to match your implementation
    }
    
    [TestMethod]
    public void TestCreateMeetUp_ReturnsOk_WhenValidUserIdAndValidMeetupDtoProvided()
    {
        // Arrange – Use real EF context with InMemory provider
        var mockConfig = new Mock<IConfiguration>();
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockLogger = new Mock<ILogger<MeetUpController>>();
        using var context = new EfDbContext(options, _mockConfig.Object);
        // Seed test data
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.SaveChanges();
        
        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);
        
        // Arrange
        var meetUpDto = new MeetUpDetailDto
        {
            MeetUpName = "Test MeetUp", 
            DateTimeFrom = DateTime.Now, 
            DateTimeTo = DateTime.Now.AddHours(1), 
            CheckList = "Test", 
            MeetUpLocation = "Location", 
            Description = "Description"
        };
        var validUserId = 1; // Assume this user exists

        // Act
        var result = controller.CreateMeetUp(validUserId, meetUpDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(1, okResult.Value); // or whatever MeetUpId you expect

    }
    
    [TestMethod]
    public void TestCreateMeetUp_ReturnsBadRequest_WhenMeetupDtoHasInvalidData()
    {
        // Arrange – Use real EF context with InMemory provider
        var mockConfig = new Mock<IConfiguration>();
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockLogger = new Mock<ILogger<MeetUpController>>();
        using var context = new EfDbContext(options, mockConfig.Object);

        // Seed a valid user
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.SaveChanges();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        var invalidMeetUpDto = new MeetUpDetailDto
        {
            MeetUpName = "", // Invalid: empty name
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Test",
            MeetUpLocation = "Location",
            Description = "Description"
        };
        var validUserId = 1;

        // Act
        var result = controller.CreateMeetUp(validUserId, invalidMeetUpDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual("MeetUp name and description are required.", badRequestResult.Value);
    }
    
    [TestMethod]
    public void TestCreateMeetUp_ReturnsOk_WhenNoMaxParticipantsInDto()
    {
        // Arrange – Use real EF context with InMemory provider
        var mockConfig = new Mock<IConfiguration>();
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockLogger = new Mock<ILogger<MeetUpController>>();
        using var context = new EfDbContext(options, mockConfig.Object);

        // Seed test user
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.SaveChanges();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        var dto = new MeetUpDetailDto
        {
            MeetUpName = "No Max Participants Test",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Checklist",
            MeetUpLocation = "Test Location",
            Description = "Test Description",
            MaxNumberOfParticipants = 0 // This should be reset to null
        };

        var userId = 1;

        // Act
        var result = controller.CreateMeetUp(userId, dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        // Optional: Verify the data was stored with null MaxNumberOfParticipants
        var savedMeetUp = context.MeetUps.FirstOrDefault(m => m.MeetUpName == "No Max Participants Test");
        Assert.IsNotNull(savedMeetUp);
        Assert.IsNull(savedMeetUp.MaxNumberOfParticipants);
    }

    [TestMethod]
    public void TestCreateMeetUp_AddsCreatorAsParticipant_WhenMeetUpIsCreated()
    {
        // Arrange – Use real EF context with InMemory provider
        var mockConfig = new Mock<IConfiguration>();
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockLogger = new Mock<ILogger<MeetUpController>>();
        using var context = new EfDbContext(options, mockConfig.Object);

        // Seed a valid user who will create the meet-up
        var creatorUser = new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        };
        context.Users.Add(creatorUser);
        context.SaveChanges();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        // Create a valid MeetUpDetailDto without MaxNumberOfParticipants
        var meetUpDto = new MeetUpDetailDto
        {
            MeetUpName = "Test MeetUp with Creator as Participant",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Checklist",
            MeetUpLocation = "Test Location",
            Description = "Test Description"
        };

        var userId = creatorUser.UserId;

        // Act: Create the meet-up
        var result = controller.CreateMeetUp(userId, meetUpDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        // Retrieve the created meet-up from the database
        var createdMeetUp = context.MeetUps.FirstOrDefault(m => m.MeetUpName == "Test MeetUp with Creator as Participant");
        Assert.IsNotNull(createdMeetUp);
        
        // Check if the creator is added as a participant
        var participant = context.Participations.FirstOrDefault(p => p.UserId == creatorUser.UserId && p.MeetUpId == createdMeetUp.MeetUpId);
        Assert.IsNotNull(participant);
    }



    [TestMethod]
    public void TestGetMeetUpDetails_ReturnsBadRequest_WhenUserIdEqualsZero()
    {
        // Arrange – Use real EF context with InMemory provider
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        // Seed test data
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Check",
            MeetUpLocation = "Somewhere",
            Description = "Test"
        });
        context.Participations.Add(new Participation { UserId = 1, MeetUpId = 1 });
        context.SaveChanges();

        // Logger und Config können gemockt bleiben
        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        // Setup fake claims (simulate JWT)
        const int userId = 0;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        // Act
        var result = controller.GetMeetUpDetails(1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsInstanceOfType(badRequestResult.Value, typeof(string)); // Assuming the value is a string message
        Assert.AreEqual("UserId invalid", badRequestResult.Value); // Adjust the message as per your implementation
    }

    [TestMethod]
    public void TestGetMeetUpDetails_ReturnsBadRequest_WhenUserIdIsBelowZero()
    {
        // Arrange – Use real EF context with InMemory provider
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        // Seed test data
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Check",
            MeetUpLocation = "Somewhere",
            Description = "Test"
        });
        context.Participations.Add(new Participation { UserId = 1, MeetUpId = 1 });
        context.SaveChanges();

        // Logger und Config können gemockt bleiben
        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        // Setup fake claims (simulate JWT)
        const int userId = -1;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        // Act
        var result = controller.GetMeetUpDetails(1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsInstanceOfType(badRequestResult.Value, typeof(string)); // Assuming the value is a string message
        Assert.AreEqual("UserId invalid", badRequestResult.Value); // Adjust the message as per your implementation
    }
    
    [TestMethod]
    public void TestGetMeetUpDetails_ReturnsBadRequest_WhenMeetUpIdEqualsZero()
    {
        // Arrange – Use real EF context with InMemory provider
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        // Seed test data
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Check",
            MeetUpLocation = "Somewhere",
            Description = "Test"
        });
        context.Participations.Add(new Participation { UserId = 1, MeetUpId = 0 });
        context.SaveChanges();

        // Logger und Config können gemockt bleiben
        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        // Setup fake claims (simulate JWT)
        const int userId = 1;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        // Act
        var result = controller.GetMeetUpDetails(0);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsInstanceOfType(badRequestResult.Value, typeof(string)); // Assuming the value is a string message
        Assert.AreEqual("MeetUpId invalid", badRequestResult.Value); // Adjust the message as per your implementation
    }
    
    [TestMethod]
    public void TestGetMeetUpDetails_ReturnsBadRequest_WhenMeetUpIdIsBelowZero()
    {
        // Arrange – Use real EF context with InMemory provider
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        // Seed test data
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Check",
            MeetUpLocation = "Somewhere",
            Description = "Test"
        });
        context.Participations.Add(new Participation { UserId = 1, MeetUpId = 1 });
        context.SaveChanges();

        // Logger und Config können gemockt bleiben
        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        // Setup fake claims (simulate JWT)
        const int userId = 1;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        // Act
        var result = controller.GetMeetUpDetails(-3);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsInstanceOfType(badRequestResult.Value, typeof(string)); // Assuming the value is a string message
        Assert.AreEqual("MeetUpId invalid", badRequestResult.Value); // Adjust the message as per your implementation
    }


    [TestMethod]
    public void TestGetMeetUpDetails_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Check",
            MeetUpLocation = "Somewhere",
            Description = "Test"
        });

        context.Participations.Add(new Participation { UserId = 99, MeetUpId = 1 });
        context.SaveChanges();

        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();
        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "99") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = controller.GetMeetUpDetails(1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }


    [TestMethod]
    public void TestGetMeetUpDetails_ReturnsNotFound_WhenMeetUpDoesNotExist()
    {
        // Arrange – Use real EF context with InMemory provider
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        // Seed test data
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Check",
            MeetUpLocation = "Somewhere",
            Description = "Test"
        });
        context.Participations.Add(new Participation { UserId = 1, MeetUpId = 2 });
        context.SaveChanges();

        // Logger und Config können gemockt bleiben
        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        // Setup fake claims (simulate JWT)
        const int userId = 1;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        // Act
        var result = controller.GetMeetUpDetails(1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        var notFoundResult = result.Result as NotFoundResult;
        Assert.IsNotNull(notFoundResult);
    }

    [TestMethod]
    public void TestGetMeetUpDetails_ReturnsOk_WhenMeetUpForUserIsFound()
    {
        // Arrange – Use real EF context with InMemory provider
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        // Seed test data
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Check",
            MeetUpLocation = "Somewhere",
            Description = "Test"
        });
        context.Participations.Add(new Participation { UserId = 1, MeetUpId = 1 });
        context.SaveChanges();

        // Logger und Config können gemockt bleiben
        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        // Setup fake claims (simulate JWT)
        const int userId = 1;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        // Act
        var result = controller.GetMeetUpDetails(1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var dto = okResult.Value as MeetUpDetailDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual(1, dto.MeetUpId);
    }

    [TestMethod]
    public void TestGetMeetUpDetails_ReturnsNotFound_WhenUserHasNoParticipation()
    {
        // Arrange – Use real EF context with InMemory provider
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        // Seed test data
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Check",
            MeetUpLocation = "Somewhere",
            Description = "Test"
        });
        
        context.SaveChanges();

        // Logger und Config können gemockt bleiben
        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        // Setup fake claims (simulate JWT)
        const int userId = 1;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        // Act
        var result = controller.GetMeetUpDetails(1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void TestGetMeetUps_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);
        
        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();
        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "99") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = controller.GetMeetUps(DateTime.Today);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        var notFound = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFound);
        StringAssert.Contains(notFound.Value?.ToString(), "does not exist");
    }


    [TestMethod]
    public void TestGetMeetUps_ReturnsNotFound_WhenNoMeetUpsExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);

        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "None",
            UserPassword = "Password123",
            ProfilePicturePath = "img.png"
        });

        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Today.AddDays(-2),
            DateTimeTo = DateTime.Today.AddDays(-1),
            CheckList = "Checklist",
            MeetUpLocation = "Here",
            Description = "Old Event"
        });

        context.Participations.Add(new Participation { UserId = 1, MeetUpId = 1 });
        context.SaveChanges();

        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();

        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);

        // Setup fake claims (simulate JWT)
        const int userId = 1;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        // Act
        var result = controller.GetMeetUps(DateTime.Today);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        var notFound = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFound);
        StringAssert.Contains(notFound.Value?.ToString(), "No meetups found");
    }
    
         [TestMethod]
    public void TestGetMeetUps_ReturnsBadRequest_WhenUserIdEqualsZero()
    {
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);
        
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Check",
            MeetUpLocation = "Somewhere",
            Description = "Test"
        });
        context.Participations.Add(new Participation { UserId = 1, MeetUpId = 1 });
        context.SaveChanges();
        
        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();
        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);
        
        // Setup fake claims (simulate JWT)
        const int userId = 0;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        var result = controller.GetMeetUps(DateTime.Today);
        
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsInstanceOfType(badRequestResult.Value, typeof(string));
        Assert.AreEqual("UserId invalid", badRequestResult.Value);
    }

    [TestMethod]
    public void TestGetMeetUps_ReturnsBadRequest_WhenUserIdBelowZero()
    {
        var options = new DbContextOptionsBuilder<EfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new EfDbContext(options, _mockConfig.Object);
        
        context.Users.Add(new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@test.com",
            DietaryRestrictions = "TestDietary",
            UserPassword = "TestPassword",
            ProfilePicturePath = "TestProfilePicturePath"
        });
        context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test MeetUp",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
            CheckList = "Check",
            MeetUpLocation = "Somewhere",
            Description = "Test"
        });
        context.Participations.Add(new Participation { UserId = 1, MeetUpId = 1 });
        context.SaveChanges();
        
        var mockLogger = new Mock<ILogger<MeetUpController>>();
        var mockConfig = new Mock<IConfiguration>();
        var controller = new MeetUpController(mockLogger.Object, mockConfig.Object, context);
        
        // Setup fake claims (simulate JWT)
        const int userId = -1;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        
        var result = controller.GetMeetUps(DateTime.Today);
        
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsInstanceOfType(badRequestResult.Value, typeof(string));
        Assert.AreEqual("UserId invalid", badRequestResult.Value);
    }
}