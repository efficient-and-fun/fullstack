using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApi.Api.Common;
using WebApi.Api.Model;

namespace WebApi;

/// <summary>
/// Defines authentication-related operations such as login, registration, and user retrieval.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Attempts to authenticate a user using the provided email and password.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>
    /// An <see cref="AuthResult"/> indicating whether authentication was successful.
    /// </returns>
    Task<AuthResult> LoginAsync(string email, string password);
    /// <summary>
    /// Registers a new user with the specified credentials and profile information.
    /// </summary>
    /// <param name="email">The email address for the new account.</param>
    /// <param name="password">The password for the new account.</param>
    /// <param name="username">The desired username.</param>
    /// <param name="profilePicturePath">The path to the user's profile picture.</param>
    /// <returns>
    /// An <see cref="AuthResult"/> indicating whether registration was successful.
    /// </returns>
    Task<AuthResult> RegisterAsync(string email, string password, string username, string profilePicturePath);
    /// <summary>
    /// Extracts the user ID from the current JWT token if available.
    /// </summary>
    /// <returns>
    /// The user ID if found in the token; otherwise, <c>null</c>.
    /// </returns>
    int? GetUserIdFromToken();
    /// <summary>
    /// Retrieves the currently authenticated user based on the token.
    /// </summary>
    /// <returns>
    /// The <see cref="User"/> object if authenticated; otherwise, <c>null</c>.
    /// </returns>
    Task<User?> GetCurrentUserAsync();
}

/// <summary>
/// Provides authentication-related functionality such as registration, login,
/// and access to the current user based on JWT tokens.
/// </summary>
public class AuthService : IAuthService
{
    private readonly EfDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="context">Entity Framework database context.</param>
    /// <param name="httpContextAccessor">Accessor for the current HTTP context to extract JWT claims.</param>
    public AuthService(EfDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Registers a new user with the given email, password, username, and optional profile picture path.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The password for the new account.</param>
    /// <param name="username">The desired username.</param>
    /// <param name="profilePicturePath">The path to the user's profile picture.</param>
    /// <returns>
    /// An <see cref="AuthResult"/> indicating success or failure and a JWT token if successful.
    /// </returns>
    public async Task<AuthResult> RegisterAsync(
        string email, 
        string password, 
        string username,
        string profilePicturePath)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            return new AuthResult
            {
                Success = false,
                Token = null,
                ErrorMessage = "Email already registered."
            };
        }

        var existingUsername = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (existingUsername != null)
        {
            return new AuthResult
            {
                Success = false,
                Token = null,
                ErrorMessage = "Username already registered."
            };
        }
        
        if (string.IsNullOrWhiteSpace(profilePicturePath))
        {
            profilePicturePath = "https://www.rainforest-alliance.org/wp-content/uploads/2021/06/capybara-square-1-400x400.jpg.optimal.jpg"; 
        }

        var newUser = new User
        {
            Email = email,
            UserPassword = BCrypt.Net.BCrypt.HashPassword(password),
            UserName = username,
            ProfilePicturePath = profilePicturePath
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        
        var token = GenerateJwtToken(newUser.UserId, email);

        return new AuthResult
        {
            Success = true,
            Token = token
        };
    }

    /// <summary>
    /// Attempts to authenticate the user using the provided credentials.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>
    /// An <see cref="AuthResult"/> indicating success or failure and a JWT token if successful.
    /// </returns>
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (foundUser == null || !BCrypt.Net.BCrypt.Verify(password, foundUser.UserPassword))
        {
            return new AuthResult { Success = false };
        }

        return new AuthResult
        {
            Success = true,
            Token = GenerateJwtToken(foundUser.UserId, foundUser.Email)
        };
    }
    
    /// <summary>
    /// Retrieves the user ID from the current JWT token if available.
    /// </summary>
    /// <returns>
    /// The user ID as an <see cref="int"/> if present in the token, otherwise <c>null</c>.
    /// </returns>
    public int? GetUserIdFromToken()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }
    
    /// <summary>
    /// Gets the currently authenticated user based on the JWT token.
    /// </summary>
    /// <returns>
    /// The <see cref="User"/> object if found, otherwise <c>null</c>.
    /// </returns>
    public async Task<User?> GetCurrentUserAsync()
    {
        var userId = GetUserIdFromToken();
        if (userId.HasValue)
        {
            return await _context.Users.FindAsync(userId.Value);
        }

        return null;
    }
    
    private string GenerateJwtToken(int userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>
/// Represents the result of an authentication operation, such as login or registration.
/// </summary>
public class AuthResult
{
    /// <summary>
    /// Indicates whether the authentication operation was successful.
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// The JWT token issued if the operation was successful, otherwise <c>null</c>.
    /// </summary>
    public string? Token { get; set; }
    /// <summary>
    /// An error message describing why the operation failed, <c>null</c> if successful.
    /// </summary>
    public string? ErrorMessage { get; set; }
}