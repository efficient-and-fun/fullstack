using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Api.Common;
using WebApi.Model;

namespace WebApi;

[ApiController, Route("api/meetup")]
public class MeetUpController : BaseController
{
    public MeetUpController(ILogger<MeetUpController> logger, IConfiguration configuration, EfDbContext context) : base(logger, configuration, context) { }
    
    /// <summary>
    /// Get MeetUps of a specified user (both accepted invitations and not accepted ones). 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="meetupId"></param>
    /// <returns></returns>
    [HttpGet, Route("{userId:int}/{meetupId:int}")]
    public async Task<ActionResult<MeetUps>> GetMeetUpDetails(int userId, [FromRoute] int meetupId)
    {
        var foundMeetUp = (from m in _context.MeetUps
            join p in _context.Participations
                on m.MeetUpId equals p.MeetUpId
            join u in _context.Users
                on p.UserId equals u.UserId
            where u.UserId == userId
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

    [HttpGet, Route("{userid:int}")]
    public ActionResult<IEnumerable<MeetUpBreefDto>> GetMeetUps(int userId)
    {
        // TODO: get meetups of users
        
        // var meetups = 
        
        return Ok();
    }
    
    [HttpGet, Route("next")]
    public ActionResult<IEnumerable<MeetUpBreefDto>> GetNextUpcomingMeetUp(DateTime currentDateTime)
    {
        // TODO: get the nearest (zeitmässig gemeint) upcoming meetup from the CURRENTDATETIME.
        
        return Ok(new MeetUpBreefDto());
    }
    
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