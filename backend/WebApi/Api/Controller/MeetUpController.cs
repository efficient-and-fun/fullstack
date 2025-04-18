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
    public ActionResult<MeetUps> GetMeetUpDetails(int userId, [FromRoute] int meetupId)
    {
        var foundMeetUp = (from m in _context.MeetUps
            join p in _context.Participations
                on m.MeetUpId equals p.MeetUpId
            join u in _context.Users
                on p.UserId equals u.UserId
            where u.UserId == userId && m.MeetUpId == meetupId
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
    [HttpGet, Route("{userId:int}")]
    public ActionResult<IEnumerable<MeetUpBriefDto>> GetMeetUps(int userId, DateTime currentDate)
    {
        var meetUps = (from m in _context.MeetUps
            join p in _context.Participations
                on m.MeetUpId equals p.MeetUpId
            where p.UserId == userId && m.DateTimeFrom >= currentDate && m.DateTimeTo <= currentDate
            select new MeetUpBriefDto()
            {
                MeetUpId = m.MeetUpId,
                MeetUpName = m.MeetUpName,
                DateTimeFrom = m.DateTimeFrom,
                DateTimeTo = m.DateTimeTo
            }).ToList();
        
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