using Microsoft.AspNetCore.Authorization;

namespace WebApi;
using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Common;
using WebApi.Model;

[ApiController, Route("api/user")]
public class UserController : BaseController
{
    private readonly IAuthService _authService;
    
    public UserController(ILogger<MeetUpController> logger, IConfiguration configuration, EfDbContext context) : base(
        logger, configuration, context)
    {
        _authService = new AuthService(context);
    }
    
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (result.Success)
        {
            return Ok(new { token = result.Token });
        }

        return Unauthorized(new { message = "Invalid credentials" });
    }
    
    [Authorize]
    [HttpPost("validate")]
    public IActionResult Validate()
    {
        return Ok(new { token = Request.Headers["Authorization"].ToString() });
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}