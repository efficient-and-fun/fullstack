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
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request.Password != request.Password2)
        {
            return BadRequest(new { message = "Passwords do not match." });
        }

        if (!request.IsAGBAccepted)
        {
            return BadRequest(new { message = "AGB must be accepted." });
        }

        var result = await _authService.RegisterAsync(
            request.Email,
            request.Password,
            //request.Vorname,
            //request.Nachname,
            request.Username
        );

        if (result.Success)
        {
            return Ok(new { token = result.Token });
        }

        return BadRequest(new { message = result.ErrorMessage ?? "Registration failed." });
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

public class RegisterRequest
{
    //public string Vorname { get; set; }
    //public string Nachname { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Password2 { get; set; }
    public bool IsAGBAccepted { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}