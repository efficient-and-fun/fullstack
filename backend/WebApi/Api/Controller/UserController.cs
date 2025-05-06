using Microsoft.AspNetCore.Authorization;

namespace WebApi;
using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Common;
using WebApi.Model;

[ApiController, Route("api/user")]
public class UserController : BaseController
{
    private readonly IAuthService _authService;
    
    public UserController(ILogger<UserController> logger, IConfiguration configuration, EfDbContext context, IAuthService authService) : base(
        logger, configuration, context)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (request.Password != request.Password2)
        {
            return BadRequest(new ErrorResponse{ Message = "Passwords do not match." });
        }

        if (!request.IsAGBAccepted)
        {
            return BadRequest(new ErrorResponse{ Message = "AGB must be accepted." });
        }

        var authResult = await _authService.RegisterAsync(request.Email, request.Password, request.Username);
        if (!authResult.Success)
        {
            return BadRequest(new ErrorResponse{ Message = authResult.ErrorMessage });
        }

        return Ok(new RegisterResponse { Token = authResult.Token });
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
        return Ok(Request.Headers.Authorization.ToString());
    }
}

public class RegisterRequest
{
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

public class RegisterResponse
{
    public string Token { get; set; }
}

public class ErrorResponse
{
    public string Message { get; set; }
}