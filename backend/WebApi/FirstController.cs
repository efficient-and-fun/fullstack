using Microsoft.AspNetCore.Mvc;

namespace WebApi;

[ApiController, Route("api/first")]
public class FirstController : BaseController
{
    public FirstController()
    {
        
    }

    public class EchoDto
    {
        public string? Message { get; set; }
    }

    [HttpPost, Route("echo")]
    public ActionResult<string> Echo([FromBody] EchoDto dto)
    {
        return Ok(dto.Message);
    }
}