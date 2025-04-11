using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebApi;

[ApiController, Route("api/meetup")]
public class MeetUpController
{
    public MeetUpController()
    {
        
    }
    
    
    [HttpGet, Route("{id}")]
    public ActionResult<string> GetDetails1([FromBody] FirstController.EchoDto dto)
    {
        return Ok("");
    }
    
    [HttpPost, Route("{id}")]
    public ActionResult<string> GetDetails2([FromBody] FirstController.EchoDto dto)
    {
        return Ok("");
    }
}