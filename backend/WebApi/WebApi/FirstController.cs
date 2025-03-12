using Microsoft.AspNetCore.Mvc;

namespace WebApi;

[ApiController, Route("api/first")]
public class FirstController : BaseController
{
    public FirstController()
    {
        
    }

    [HttpGet, Route("api/[controller]/hello")]
    public ActionResult<string> GetHello()
    {
        return Ok("Hello");
    }
}