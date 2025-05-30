using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using WebApi.Api.Common;
using WebApi.Api.Model;

namespace WebApi.Api.Controller;

[ApiController, Route("api/users")]
public class UserController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IConfiguration configuration, EfDbContext context, IAuthService authService, IUserService userService) : base(logger, configuration, context)
    {
        _authService = authService;
        _userService = userService;
    }

    /// <summary>
    /// Get a list of users.
    /// </summary>
    /// <returns>List of <see cref="UserDto"/> objects.</returns>
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await Context.Users
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                ProfilePicturePath = u.ProfilePicturePath
            }).ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Get a list of friends that belong to the current user.
    /// </summary>
    /// <returns>List of <see cref="UserDto"/> objects.</returns>
    [Authorize]
    [HttpGet("friends")]
    public async Task<ActionResult<List<UserDto>>> GetFriends()
    {
        var userId = _authService.GetUserIdFromToken();
        if (userId == null)
            return Unauthorized();

        var friends = await Context.FriendConnection
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

    /// <summary>
    /// Add a friend to a user.
    /// </summary>
    /// <param name="friendName">Username of the friend that is to be added.</param>
    /// <returns>
    /// Returns <see cref="OkObjectResult"/> with result on success, 
    /// <see cref="UnauthorizedResult"/> if the user is not authenticated,
    /// or <see cref="BadRequestObjectResult"/> if the operation fails.
    /// </returns>
    [Authorize]
    [HttpPost("friends")]
    public async Task<ActionResult> AddFriend([FromQuery] string friendName)
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

    /// <summary>
    /// Remove a friend of a user.
    /// </summary>
    /// <param name="friendName">Username of the friend that is to be removed.</param>
    /// <returns>
    /// Returns <see cref="OkObjectResult"/> with result on success, 
    /// <see cref="UnauthorizedResult"/> if the user is not authenticated,
    /// or <see cref="BadRequestObjectResult"/> if the operation fails.
    /// </returns>
    [Authorize]
    [HttpDelete("friends")]
    public async Task<ActionResult> RemoveFriend([FromQuery] string friendName)
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

    /// <summary>
    /// Register a new user based on provided credentials.
    /// </summary>
    /// <param name="request">
    /// Request data in the form of a <see cref="RegisterRequest"/>. Provided by the user.
    /// </param>
    /// <returns>
    /// Returns <see cref="OkObjectResult"/> with auth token on success
    /// or <see cref="BadRequestObjectResult"/> with the result being of type <see cref="ErrorResponse"/>,
    /// which gives detailed information about why the registration has failed.
    /// </returns>
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

    /// <summary>
    /// Login an existing user based on provided credentials.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Returns <see cref="OkObjectResult"/> with auth token on success,
    /// <see cref="BadRequestObjectResult"/> with the result being of type <see cref="ErrorResponse"/> if the login has failed based on invalid request data
    /// or <see cref="UnauthorizedObjectResult"/> with the result being of type <see cref="ErrorResponse"/> if the login has failed based on invalid credentials.
    /// </returns>
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

    /// <summary>
    /// Endpoint to validate whether a user is currently logged in.
    /// </summary>
    /// <returns>
    /// Returns <see cref="OkObjectResult"/> with the value of the Authorization header if the user is authenticated.
    /// </returns>
    [Authorize]
    [HttpPost("validate")]
    public IActionResult Validate()
    {
        return Ok(Request.Headers.Authorization.ToString());
    }
}