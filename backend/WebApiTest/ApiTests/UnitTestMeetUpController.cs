using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi;
using WebApi.Api.Common;
using WebApi.Api.Controller;
using WebApi.Api.Model;

namespace WebApiTest.ApiTests;

[TestClass]
public class MeetUpControllerTests
{
    private Mock<ILogger<MeetUpController>> _loggerMock = null!;
    private Mock<IConfiguration> _configMock = null!;
    private EfDbContext _context = null!;
    private Mock<IAuthService> _authServiceMock = null!;
    private MeetUpController _controller = null!;
    private User testUser;

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

        _controller = new MeetUpController(_loggerMock.Object, _configMock.Object, _context, _authServiceMock.Object);
        testUser = new User
        {
            UserId = 1,
            Email = "testuser@example.com",
            ProfilePicturePath = "/images/default.png",
            UserName = "testuser",
            UserPassword = "securepassword123"
        };
    }

    [TestMethod]
    public void CreateMeetUp_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns((int?)null);

        var result = _controller.CreateMeetUp(new MeetUpCreateDto());

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    public void CreateMeetUp_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(testUser.UserId);

        var result = _controller.CreateMeetUp(new MeetUpCreateDto
        {
            MeetUpName = "Test",
            Description = "Valid description",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1)
        });

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public void CreateMeetUp_ReturnsBadRequest_WhenInvalidInput()
    {
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(testUser.UserId);
        _context.Users.Add(testUser);
        _context.SaveChanges();

        var result = _controller.CreateMeetUp(new MeetUpCreateDto());

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void CreateMeetUp_ReturnsOk_WhenInputIsValid()
    {
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(testUser.UserId);
        _context.Users.Add(testUser);
        _context.SaveChanges();

        var dto = new MeetUpCreateDto
        {
            MeetUpName = "Test MeetUp",
            Description = "Test description",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
        };

        var result = _controller.CreateMeetUp(dto);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void UpdateMeetUp_ReturnsForbidden_WhenUserNotParticipant()
    {
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(testUser.UserId);
        _context.Users.Add(testUser);
        _context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test",
            Description = "Test",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
        });
        _context.SaveChanges();

        var result = _controller.UpdateMeetUp(1, new MeetUp
        {
            MeetUpId = 1,
            MeetUpName = "Updated",
            Description = "Updated",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1),
        });

        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }

    [TestMethod]
    public void GetMeetUpDetails_ReturnsNotFound_WhenMeetupNotFound()
    {
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(testUser.UserId);
        _context.Users.Add(testUser);
        _context.SaveChanges();

        var result = _controller.GetMeetUpDetails(999);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void GetMeetUpDetails_ReturnsOk_WhenValid()
    {
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(testUser.UserId);

        _context.Users.Add(testUser);
        _context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Test",
            Description = "Desc",
            DateTimeFrom = DateTime.Now,
            DateTimeTo = DateTime.Now.AddHours(1)
        });
        _context.Participations.Add(new Participation
        {
            MeetUpId = 1,
            UserId = testUser.UserId,
            HasAcceptedInvitation = true
        });

        _context.SaveChanges();

        var result = _controller.GetMeetUpDetails(1);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void GetMeetUps_ReturnsNotFound_WhenNoneFound()
    {
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(testUser.UserId);
        _context.Users.Add(testUser);
        _context.SaveChanges();

        var result = _controller.GetMeetUps(DateTime.Now);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public void GetMeetUps_ReturnsOk_WhenMeetUpsFound()
    {
        _authServiceMock.Setup(s => s.GetUserIdFromToken()).Returns(testUser.UserId);

        _context.Users.Add(testUser);
        _context.MeetUps.Add(new MeetUps
        {
            MeetUpId = 1,
            MeetUpName = "Event",
            Description = "Desc",
            DateTimeFrom = DateTime.Now.AddHours(-1),
            DateTimeTo = DateTime.Now.AddHours(1)
        });
        _context.Participations.Add(new Participation
        {
            UserId = testUser.UserId,
            MeetUpId = 1,
            HasAcceptedInvitation = true
        });

        _context.SaveChanges();

        var result = _controller.GetMeetUps(DateTime.Now);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var meetups = (result.Result as OkObjectResult)?.Value as IEnumerable<MeetUpBriefDto>;
        Assert.IsNotNull(meetups);
        Assert.AreEqual(1, meetups.Count());
    }
}
