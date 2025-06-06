using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Common;
using WebApi.Api.Model;

namespace WebApi.Api.Controller;
/// <summary>
/// Controller for managing MeetUps.
/// Provides endpoints to create, update, and retrieve MeetUps.
/// </summary>
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
        var (validation, userId) = ValidateCommon(_authService, Context);
        if (validation != null) return validation;

        var validationResult = ValidateMeetupDto(meetupDto);
        if (validationResult != null) return validationResult;

        var newMeetUp = new MeetUps
        {
            MeetUpName = meetupDto.MeetUpName,
            Description = meetupDto.Description,
            DateTimeFrom = meetupDto.DateTimeFrom,
            DateTimeTo = meetupDto.DateTimeTo,
            CheckList = meetupDto.CheckList,
            MeetUpLocation = meetupDto.MeetUpLocation,
            MaxNumberOfParticipants = meetupDto.MaxNumberOfParticipants > 0 ? meetupDto.MaxNumberOfParticipants : null
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
    public ActionResult UpdateMeetUp([FromRoute] int meetupId, [FromBody] MeetUp updatedMeetUp)
    {
        var (validation, userId) = ValidateCommon(_authService, Context);
        if (validation != null) return validation;

        if (meetupId <= 0) return BadRequest("MeetUpId invalid");

        var meetUp = Context.MeetUps.Find(meetupId);
        if (meetUp == null) return NotFound($"MeetUp with ID {meetupId} not found.");

        var participation = Context.Participations
            .FirstOrDefault(p => p.UserId == userId.Value && p.MeetUpId == meetupId);
        
        // TODO discuss: check if the user is the creator or authorized participant
        if (participation == null) return Forbid("User is not authorized to update this meetup.");

        var validationResult = ValidateMeetupDto(updatedMeetUp);
        if (validationResult != null) return validationResult;

        meetUp.MeetUpName = updatedMeetUp.MeetUpName;
        meetUp.DateTimeFrom = updatedMeetUp.DateTimeFrom;
        meetUp.DateTimeTo = updatedMeetUp.DateTimeTo;
        meetUp.Description = updatedMeetUp.Description;
        meetUp.CheckList = updatedMeetUp.CheckList;
        meetUp.MeetUpLocation = updatedMeetUp.MeetUpLocation;
        meetUp.MaxNumberOfParticipants = updatedMeetUp.MaxNumberOfParticipants > 0 ? updatedMeetUp.MaxNumberOfParticipants : null;

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
        var (validation, userId) = ValidateCommon(_authService, Context);
        if (validation != null) return validation;

        if (meetupId <= 0) return BadRequest("MeetUpId invalid");

        var user = Context.Users.FirstOrDefault(u => u.UserId == userId.Value);
        var meetUp = Context.MeetUps.FirstOrDefault(m => m.MeetUpId == meetupId);
        var participation = Context.Participations.FirstOrDefault(p => p.UserId == userId.Value && p.MeetUpId == meetupId);
        if (user == null || meetUp == null || participation == null) return NotFound();

        var foundMeetUp = (from m in Context.MeetUps
            join p in Context.Participations on m.MeetUpId equals p.MeetUpId
            join u in Context.Users on p.UserId equals u.UserId
            where u.UserId == userId.Value && m.MeetUpId == meetupId
            select new MeetUp
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
        var (validation, userId) = ValidateCommon(_authService, Context);
        if (validation != null) return validation;

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

        if (!meetUps.Any())
        {
            return NotFound($"No meetups found for user {userId.Value} on {currentDate:yyyy-MM-dd}.");
        }
        return Ok(meetUps);
    }

    private static ActionResult ValidateMeetupDto(dynamic dto)
    {
        if (string.IsNullOrWhiteSpace(dto.MeetUpName) || string.IsNullOrWhiteSpace(dto.Description))
            return new BadRequestObjectResult("MeetUp name and description are required.");

        if (dto.DateTimeFrom == DateTime.MinValue || dto.DateTimeTo == DateTime.MinValue)
            return new BadRequestObjectResult("MeetUp start and end times are required.");

        if (dto.DateTimeFrom >= dto.DateTimeTo)
            return new BadRequestObjectResult("MeetUp start time must be before end time.");

        return null;
    }

    private static (ActionResult? Result, int? UserId) ValidateCommon(IAuthService authService, EfDbContext context)
    {
        var userId = authService.GetUserIdFromToken();
        if (userId == null)
            return (new UnauthorizedObjectResult("User not authenticated."), null);

        var userExists = context.Users.Any(u => u.UserId == userId.Value);
        return userExists 
            ? (null, userId) 
            : (new NotFoundObjectResult($"User with ID {userId} does not exist."), null);
    }
}
