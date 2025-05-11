using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Api.Common;
using WebApi.Model;

namespace WebApi;

[ApiController, Route("api/meetups")]
public class MeetUpController : BaseController
{
    private readonly IAuthService _authService;
    public MeetUpController(ILogger<MeetUpController> logger, IConfiguration configuration, EfDbContext context,
        IAuthService authService) : base(logger, configuration, context)
    {
        _authService = authService;
    }
    private static ActionResult ValidateMeetupDetail(MeetUpDetailDto meetupDto)
    {
        // Validate if meetup name or description is empty
        if (string.IsNullOrWhiteSpace(meetupDto.MeetUpName) || string.IsNullOrWhiteSpace(meetupDto.Description))
        {
            return new BadRequestObjectResult("MeetUp name and description are required.");
        }
        // Validate if start or end times are non-default values
        if (meetupDto.DateTimeFrom == default || meetupDto.DateTimeTo == default)
        {
            return new BadRequestObjectResult("MeetUp start and end times are required.");
        }
        // Validate if start time is before end time
        if (meetupDto.DateTimeFrom >= meetupDto.DateTimeTo)
        {
            return new BadRequestObjectResult("MeetUp start time must be before end time.");
        }
        
        // If the number of participants is less than or equal to 0, set it to null
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
    
    /// <summary>
    /// Create a new MeetUp.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="meetupDto"></param>
    /// <returns>
    /// Returns 200 and the ID of the newly created MeetUp on success, 400 if input is invalid, or 404 if the user doesn't exist.
    /// </returns>
    [Authorize]
    [HttpPost]
    public ActionResult<int> CreateMeetUp([FromBody] MeetUpDetailDto meetupDto)
    {
        var userId = _authService.GetUserIdFromToken();
        if (userId == null)
        {
            return Unauthorized("User not authenticated.");
        }
        var userExists = _context.Users.Any(u => u.UserId == userId.Value);
        if (!userExists)
            return NotFound($"User with ID {userId} does not exist.");

        // Validate the input data
        var validationResult = ValidateMeetupDetail(meetupDto);
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

        _context.MeetUps.Add(newMeetUp);
        _context.SaveChanges();

        // todo discuss: Add creator as participant
        _context.Participations.Add(new Participation
        {
            UserId = userId.Value,
            MeetUpId = newMeetUp.MeetUpId,
            HasAcceptedInvitation = true
        });

        _context.SaveChanges();

        return Ok(newMeetUp.MeetUpId);
    }
    
    /// <summary>
    /// Updates a meetup's details if the user is a participant.
    /// </summary>
    /// <param name="userId">ID of the user requesting the update.</param>
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
        if (userId == null)
        {
            return Unauthorized("User not authenticated.");
        }
        var userExists = _context.Users.Any(u => u.UserId == userId.Value);
        if (!userExists)
            return NotFound($"User with ID {userId} does not exist.");
        
        // Validate the input data
        var validationMeetUpIdResult = ValidateMeetupId(meetupId);

        // Check if the meetup exists
        var meetUp = _context.MeetUps.Find(meetupId);
        if (meetUp == null)
        {
            return NotFound($"MeetUp with ID {meetupId} not found.");
        }

        // todo discuss: check if the user is the creator or authorized participant
        var participation = _context.Participations
            .FirstOrDefault(p => p.UserId == userId.Value && p.MeetUpId == meetupId);
        if (participation == null)
        {
            return Forbid("User is not authorized to update this meetup.");
        }
        
        // Validate the input data
        var validationResult = ValidateMeetupDetail(updatedMeetUp);
        if (validationResult is not OkResult)
        {
            return validationResult;
        }
        
        // todo: optional create specific update methods for only the fields that are necessary to update.

        // Update fields
        meetUp.MeetUpName = updatedMeetUp.MeetUpName;
        meetUp.DateTimeFrom = updatedMeetUp.DateTimeFrom;
        meetUp.DateTimeTo = updatedMeetUp.DateTimeTo;
        meetUp.Description = updatedMeetUp.Description;
        meetUp.CheckList = updatedMeetUp.CheckList;
        meetUp.MeetUpLocation = updatedMeetUp.MeetUpLocation;
        meetUp.MaxNumberOfParticipants = updatedMeetUp.MaxNumberOfParticipants;

        _context.MeetUps.Update(meetUp);
        _context.SaveChanges();

        return NoContent();
    }
    
    /// <summary>
    /// Get MeetUp Details for a Meetup of a specified user (both accepted invitations and not accepted ones). 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="meetupId"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet, Route("{meetupId:int}")]
    public ActionResult<MeetUps> GetMeetUpDetails([FromRoute] int meetupId)
    {
        var userId = _authService.GetUserIdFromToken();
        if (userId == null)
        {
            return Unauthorized("User not authenticated.");
        }
        var userExists = _context.Users.Any(u => u.UserId == userId.Value);
        if (!userExists)
            return NotFound($"User with ID {userId} does not exist.");
        
        if (meetupId <= 0)
        {
            return BadRequest("MeetUpId invalid");
        }
        
        var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
        var meetUp = _context.MeetUps.FirstOrDefault(m => m.MeetUpId == meetupId);
        var participation = _context.Participations.FirstOrDefault(p => p.UserId == userId.Value && p.MeetUpId == meetupId);
        if (user == null || meetUp == null || participation == null)
        {
            return NotFound();
        }
        
        var foundMeetUp = (from m in _context.MeetUps
            join p in _context.Participations
                on m.MeetUpId equals p.MeetUpId
            join u in _context.Users
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
        
        // TODO: weiteres Beispiel, remove at some later stage.
        // var foundMeetUp = await _context.MeetUps.Where(m => m.MeetUpId == meetupId).FirstOrDefaultAsync();
        
        return Ok(foundMeetUp);
    }

    /// <summary>
    /// Get all MeetUps user has a participation to for a specific day.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="currentDate"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public ActionResult<IEnumerable<MeetUpBriefDto>> GetMeetUps([FromQuery] DateTime currentDate)
    {
        var userId = _authService.GetUserIdFromToken();
        if (userId == null)
        {
            return Unauthorized("User not authenticated.");
        }
        var userExists = _context.Users.Any(u => u.UserId == userId.Value);
        if (!userExists)
            return NotFound($"User with ID {userId} does not exist.");

        var meetUps = (from m in _context.MeetUps
            join p in _context.Participations on m.MeetUpId equals p.MeetUpId
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
    
    
    // TODO: reuse this if necessary. If it is not going to be used ==> delete.
    /// <summary>
    /// Get the MeetUp the user has an invitation for and is in the future of the specified date(time).
    /// Important: format for parameter "currentDateTime" (without the quotes): "2025-04-14 00:00"
    /// </summary>
    /// <param name="currentDateTime">DateTime that the MeetUps in the database are compared to (based on MeetUps.DateTimeFrom).</param>
    /// <returns>MeetUpBriefDto object that the specified user has an invitation for and also lies in the future based on the provided "currentDateTime".</returns>
    // [HttpGet, Route("next")]
    // public ActionResult<MeetUpBriefDto> GetNextUpcomingMeetUp(DateTime currentDateTime)
    // {
    //     var now = currentDateTime;
    //     var futureMeetUp = (from m in _context.MeetUps
    //         join p in _context.Participations on m.MeetUpId equals p.MeetUpId
    //     orderby m.DateTimeFrom
    //             where m.DateTimeFrom >= now
    //         select new MeetUpBriefDto()
    //         {
    //             MeetUpId = m.MeetUpId,
    //             MeetUpName = m.MeetUpName,
    //             Description = m.Description,
    //             DateTimeFrom = m.DateTimeFrom,
    //             DateTimeTo = m.DateTimeTo
    //         }).FirstOrDefault();
    //     
    //     return Ok(futureMeetUp);
    // }
    
    // TODO: Readd this in the next sprint where this feature is actually added.
    // [HttpPost, Route("")]
    // public ActionResult<string> CreateMeetup(int userId, MeetUpDto meetup)
    // {
    //     
    //     
    //     // TODO: insert into db
    //     var newMeetUpId = -1;
    //     return Ok(newMeetUpId);
    // }
}