using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Common;
using WebApi.Model;

namespace WebApi.Api.Controller;

[ApiController, Route("api/meetups")]
public class MeetUpController : BaseController
{
    private readonly IAuthService _authService;
    public MeetUpController(ILogger<MeetUpController> logger, IConfiguration configuration, EfDbContext context, IAuthService authService) : base(logger, configuration, context)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// Create a new MeetUp.
    /// </summary>
    /// <param name="meetupDto"></param>
    /// <returns>
    /// Returns 200 and the ID of the newly created MeetUp on success, 400 if input is invalid, or 404 if the user doesn't exist.
    /// </returns>
    [Authorize]
    [HttpPost]
    public ActionResult<int> CreateMeetUp([FromBody] MeetUpCreateDto meetupDto)
    {
        var userId = _authService.GetUserIdFromToken();
        var actionResult = ValidateUser(userId, Context);
        if (actionResult is not OkResult)
        {
            return actionResult;
        }

        var validationResult = ValidateMeetupCreate(meetupDto);
        if (validationResult is not OkResult)
        {
            return validationResult;
        }
        
        var newMeetUp = new MeetUps
        {
            MeetUpName = meetupDto.MeetUpName,
            Description = meetupDto.Description,
            DateTimeFrom = meetupDto.DateTimeFrom,
            DateTimeTo = meetupDto.DateTimeTo,
            CheckList = meetupDto.CheckList,
            MeetUpLocation = meetupDto.MeetUpLocation,
            MaxNumberOfParticipants = meetupDto.MaxNumberOfParticipants
        };

        Context.MeetUps.Add(newMeetUp);
        Context.SaveChanges();

        // TODO discuss: Add creator as participant
        Context.Participations.Add(new Participation
        {
            UserId = userId.Value,
            MeetUpId = newMeetUp.MeetUpId,
            HasAcceptedInvitation = true
        });

        Context.SaveChanges();

        return Ok(newMeetUp.MeetUpId);
    }
    
    /// <summary>
    /// Updates a meetup's details if the user is a participant.
    /// </summary>
    /// <param name="meetupId">ID of the meetup to update.</param>
    /// <param name="updatedMeetUp">The updated data for the meetup.</param>
    /// <returns>
    /// Returns 204 No Content on success, 400 if input is invalid, 404 if the user or meetup doesn't exist,
    /// or 403 if the user is not a participant.
    /// </returns>
    [Authorize]
    [HttpPut, Route("{meetupId:int}")]
    public ActionResult UpdateMeetUp([FromRoute] int meetupId, [FromBody] MeetUpDetailDto updatedMeetUp)
    {
        var userId = _authService.GetUserIdFromToken();
        var actionResult = ValidateUser(userId, Context);
        if (actionResult is not OkResult)
        {
            return actionResult;
        }
        
        var validationMeetUpIdResult = ValidateMeetupId(meetupId);
        if (validationMeetUpIdResult is not OkResult)
        {
            return validationMeetUpIdResult;
        }

        var meetUp = Context.MeetUps.Find(meetupId);
        if (meetUp == null)
        {
            return NotFound($"MeetUp with ID {meetupId} not found.");
        }

        // TODO discuss: check if the user is the creator or authorized participant
        var participation = Context.Participations
            .FirstOrDefault(p => p.UserId == userId.Value && p.MeetUpId == meetupId);
        if (participation == null)
        {
            return Forbid("User is not authorized to update this meetup.");
        }
        
        var validationResult = ValidateMeetupUpdate(updatedMeetUp);
        if (validationResult is not OkResult)
        {
            return validationResult;
        }
        
        meetUp.MeetUpName = updatedMeetUp.MeetUpName;
        meetUp.DateTimeFrom = updatedMeetUp.DateTimeFrom;
        meetUp.DateTimeTo = updatedMeetUp.DateTimeTo;
        meetUp.Description = updatedMeetUp.Description;
        meetUp.CheckList = updatedMeetUp.CheckList;
        meetUp.MeetUpLocation = updatedMeetUp.MeetUpLocation;
        meetUp.MaxNumberOfParticipants = updatedMeetUp.MaxNumberOfParticipants;

        Context.MeetUps.Update(meetUp);
        Context.SaveChanges();

        return NoContent();
    }
    
    /// <summary>
    /// Get MeetUp Details for a Meetup of a specified user (both accepted invitations and not accepted ones). 
    /// </summary>
    /// <param name="meetupId"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet, Route("{meetupId:int}")]
    public ActionResult<MeetUps> GetMeetUpDetails([FromRoute] int meetupId)
    {
        var userId = _authService.GetUserIdFromToken();
        var actionResult = ValidateUser(userId, Context);
        if (actionResult is not OkResult)
        {
            return actionResult;
        }
        
        if (meetupId <= 0)
        {
            return BadRequest("MeetUpId invalid");
        }
        
        var user = Context.Users.FirstOrDefault(u => u.UserId == userId.Value);
        var meetUp = Context.MeetUps.FirstOrDefault(m => m.MeetUpId == meetupId);
        var participation = Context.Participations.FirstOrDefault(p => p.UserId == userId.Value && p.MeetUpId == meetupId);
        if (user == null || meetUp == null || participation == null)
        {
            return NotFound();
        }
        
        var foundMeetUp = (from m in Context.MeetUps
            join p in Context.Participations
                on m.MeetUpId equals p.MeetUpId
            join u in Context.Users
                on p.UserId equals u.UserId
            where u.UserId == userId.Value && m.MeetUpId == meetupId
            select new MeetUpDetailDto()
            {
                MeetUpId = m.MeetUpId,
                MeetUpName = m.MeetUpName,
                DateTimeFrom = m.DateTimeFrom,
                DateTimeTo = m.DateTimeTo,
                CheckList = m.CheckList,
                MeetUpLocation = m.MeetUpLocation,
                Description = m.Description,
            }).FirstOrDefault();
        
        return Ok(foundMeetUp);
    }

    /// <summary>
    /// Get all MeetUps user has a participation to for a specific day.
    /// </summary>
    /// <param name="currentDate"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public ActionResult<IEnumerable<MeetUpBriefDto>> GetMeetUps([FromQuery] DateTime currentDate)
    {
        var userId = _authService.GetUserIdFromToken();
        var actionResult = ValidateUser(userId, Context);
        if (actionResult is not OkResult)
        {
            return actionResult;
        }

        var meetUps = (from m in Context.MeetUps
            join p in Context.Participations on m.MeetUpId equals p.MeetUpId
            where p.UserId == userId.Value
                  && m.DateTimeFrom.Date <= currentDate.Date
                  && m.DateTimeTo.Date >= currentDate.Date
            select new MeetUpBriefDto
            {
                MeetUpId = m.MeetUpId,
                MeetUpName = m.MeetUpName,
                DateTimeFrom = m.DateTimeFrom,
                DateTimeTo = m.DateTimeTo
            }).ToList();

        if (meetUps.Count == 0)
        {
            return NotFound($"No meetups found for user {userId.Value} on {currentDate:yyyy-MM-dd}.");
        }
        return Ok(meetUps);
    }
    
    private static ActionResult ValidateMeetupCreate(MeetUpCreateDto meetupDto)
    {
        if (string.IsNullOrWhiteSpace(meetupDto.MeetUpName) || string.IsNullOrWhiteSpace(meetupDto.Description))
        {
            return new BadRequestObjectResult("MeetUp name and description are required.");
        }
        
        if (meetupDto.DateTimeFrom == default || meetupDto.DateTimeTo == default)
        {
            return new BadRequestObjectResult("MeetUp start and end times are required.");
        }

        if (meetupDto.DateTimeFrom >= meetupDto.DateTimeTo)
        {
            return new BadRequestObjectResult("MeetUp start time must be before end time.");
        }
        
        if (meetupDto.MaxNumberOfParticipants <= 0)
        {
            meetupDto.MaxNumberOfParticipants = null;
        }
        
        return new OkResult();
    }
    
    private static ActionResult ValidateMeetupUpdate(MeetUpDetailDto meetupDto)
    {
        if (string.IsNullOrWhiteSpace(meetupDto.MeetUpName) || string.IsNullOrWhiteSpace(meetupDto.Description))
        {
            return new BadRequestObjectResult("MeetUp name and description are required.");
        }

        if (meetupDto.DateTimeFrom == default || meetupDto.DateTimeTo == default)
        {
            return new BadRequestObjectResult("MeetUp start and end times are required.");
        }

        if (meetupDto.DateTimeFrom >= meetupDto.DateTimeTo)
        {
            return new BadRequestObjectResult("MeetUp start time must be before end time.");
        }
        
        if (meetupDto.MaxNumberOfParticipants <= 0)
        {
            meetupDto.MaxNumberOfParticipants = null;
        }
        
        return new OkResult();
    }

    private static ActionResult ValidateMeetupId(int meetupId)
    {
        if (meetupId <= 0)
        {
            return new BadRequestObjectResult("MeetUpId invalid");
        }
        return new OkResult();
    }
    
    private static ActionResult ValidateUser(int? userId, EfDbContext context)
    {
        if (userId == null)
        {
            return new UnauthorizedObjectResult("User not authenticated.");
        }

        var userExists = context.Users.Any(u => u.UserId == userId.Value);
        if (!userExists)
        {
            return new NotFoundObjectResult($"User with ID {userId} does not exist.");
        }

        return new OkResult();
    }
}