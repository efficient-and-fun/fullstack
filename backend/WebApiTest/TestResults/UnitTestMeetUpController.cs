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
            DiataryRestrictions = "TestDiatary",
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

        // Act
        var result = controller.GetMeetUpDetails(0, 1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsInstanceOfType(badRequestResult.Value, typeof(string)); // Assuming the value is a string message
        Assert.AreEqual("UserId invalid", badRequestResult.Value); // Adjust the message as per your implementation
    }

    [TestMethod]
    public void TestGetMeetUpDetails_ReturnsBadRequest_WhenUserIdEqualsBelowZero()
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
            DiataryRestrictions = "TestDiatary",
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

        // Act
        var result = controller.GetMeetUpDetails(-3, 1);

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
            DiataryRestrictions = "TestDiatary",
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

        // Act
        var result = controller.GetMeetUpDetails(1, 0);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsInstanceOfType(badRequestResult.Value, typeof(string)); // Assuming the value is a string message
        Assert.AreEqual("MeetUpId invalid", badRequestResult.Value); // Adjust the message as per your implementation
    }
    
    [TestMethod]
    public void TestGetMeetUpDetails_ReturnsBadRequest_WhenMeetUpIdEqualsBelowZero()
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
            DiataryRestrictions = "TestDiatary",
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

        // Act
        var result = controller.GetMeetUpDetails(1, -3);

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
            DiataryRestrictions = "TestDiatary",
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

        // Act
        var result = controller.GetMeetUpDetails(2, 1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
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
            DiataryRestrictions = "TestDiatary",
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

        // Act
        var result = controller.GetMeetUpDetails(2, 1);

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
            DiataryRestrictions = "TestDiatary",
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

        // Act
        var result = controller.GetMeetUpDetails(1, 1);

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
            DiataryRestrictions = "TestDiatary",
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

        // Act
        var result = controller.GetMeetUpDetails(1, 1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }
}