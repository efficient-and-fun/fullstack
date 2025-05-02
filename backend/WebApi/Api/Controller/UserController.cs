namespace WebApi;
using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Common;
using WebApi.Model;

[ApiController, Route("api/user")]
public class UserController : BaseController
{
    public UserController(ILogger<MeetUpController> logger, IConfiguration configuration, EfDbContext context) : base(
        logger, configuration, context)
    {
    }

}