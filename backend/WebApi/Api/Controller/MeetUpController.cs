using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Api.Common;
using WebApi.Model;

namespace WebApi;

[ApiController, Route("api/meetup")]
public class MeetUpController : BaseController
{
    public MeetUpController(ILogger<MeetUpController> logger, IConfiguration configuration, EfDbContext context) : base(logger, configuration, context) { }
    
    [HttpGet, Route("{userId:int}/{meetupId:int}")]
    public async Task<ActionResult<MeetUps>> GetMeetUpDetails(int userId, [FromRoute] int meetupId)
    {
        var test = await _context.meetups.Where(m => m.MeetUpId == meetupId).ToListAsync();
        
        var firstMeetUp = new MeetUpDetailDto()
        {
            MeetUpId = 1,
            MeetUpName = "GameNight",
            Description = "Mega cooles Alias Event",
            MeetUpLocation = "Irgendwo in Winti",
            CheckList = "Motivation, Lust, Lebensfreude",
            DateTimeFrom = new DateTime(2020, 01, 01),
            DateTimeTo = new DateTime(2020, 01, 02)
        };
        
        return Ok(test);
    }

    [HttpGet, Route("{userid:int}")]
    public ActionResult<IEnumerable<MeetUpBreefDto>> GetMeetUps(int userId)
    {
        // TODO: get meetups of users
        
        var meetups = new List<MeetUpBreefDto>();

        meetups.Add(new MeetUpBreefDto() { MeetUpId = 1, MeetUpName = "GameNight 1", Description = "Mega cooles Alias Event 1", DateTimeFrom = new DateTime(2020, 01, 01), DateTimeTo = new DateTime(2020, 01, 02) });
        meetups.Add(new MeetUpBreefDto() { MeetUpId = 2, MeetUpName = "GameNight 2", Description = "Mega cooles Alias Event 2", DateTimeFrom = new DateTime(2020, 01, 01), DateTimeTo = new DateTime(2020, 01, 02) });
        meetups.Add(new MeetUpBreefDto() { MeetUpId = 3, MeetUpName = "GameNight 3", Description = "Mega cooles Alias Event 3", DateTimeFrom = new DateTime(2020, 01, 01), DateTimeTo = new DateTime(2020, 01, 02) });
        
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