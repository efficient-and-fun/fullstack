using Microsoft.AspNetCore.Authorization;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Common;
using WebApi.Model;

[ApiController, Route("api/users")]
public class UserController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IConfiguration configuration, EfDbContext context,
        IAuthService authService, IUserService userService) : base(
        logger, configuration, context)
    {
        _authService = authService;
        _userService = userService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                ProfilePicturePath = u.ProfilePicturePath
            }).ToListAsync();

        return Ok(users);
    }

    [Authorize]
    [HttpGet("friends")]
    public async Task<ActionResult<List<UserDto>>> GetFriends()
    {
        var userId = _authService.GetUserIdFromToken();
        if (userId == null)
            return Unauthorized();

        var friends = await _context.FriendConnection
            .Where(fc => fc.UserId == userId && fc.HasAcceptedFriendRequest)
            .Include(fc => fc.Friend)
            .Select(fc => new UserDto
            {
                UserId = fc.Friend.UserId,
                UserName = fc.Friend.UserName,
                Email = fc.Friend.Email,
                ProfilePicturePath = fc.Friend.ProfilePicturePath
            })
            .ToListAsync();

        return Ok(friends);
    }

    [Authorize]
    [HttpPost("friends")]
    public async Task<ActionResult> AddFriend(string friendName)
    {
        var userId = _authService.GetUserIdFromToken();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _userService.AddFriend(userId.Value, friendName);
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [Authorize]
    [HttpDelete("friends")]
    public async Task<ActionResult> RemoveFriend(string friendName)
    {
        var userId = _authService.GetUserIdFromToken();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _userService.RemoveFriend(userId.Value, friendName);
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse { Message = "Invalid request" });
        }

        if (request.Password != request.Password2)
        {
            return BadRequest(new ErrorResponse { Message = "Passwords do not match." });
        }

        if (!request.IsAGBAccepted)
        {
            return BadRequest(new ErrorResponse { Message = "AGB must be accepted." });
        }

        var authResult = await _authService.RegisterAsync(request.Email, request.Password, request.Username,
            request.ProfilePicturePath);
        if (!authResult.Success)
        {
            return BadRequest(new ErrorResponse { Message = authResult.ErrorMessage });
        }

        return Ok(new TokenResponse { Token = authResult.Token });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse { Message = "Invalid request" });
        }

        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (result.Success)
        {
            return Ok(new TokenResponse { Token = result.Token });
        }

        return Unauthorized(new ErrorResponse { Message = "Invalid credentials" });
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
    [Required] public string Username { get; set; }
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string Password2 { get; set; }
    [Required] public string ProfilePicturePath { get; set; }
    [Required] public bool IsAGBAccepted { get; set; }
}

public class LoginRequest
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
}

public class TokenResponse
{
    public string Token { get; set; }
}

public class ErrorResponse
{
    public string Message { get; set; }
}