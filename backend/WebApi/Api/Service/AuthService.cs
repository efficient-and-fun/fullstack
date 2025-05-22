using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApi.Api.Common;
using WebApi.Api.Model;

namespace WebApi;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string username, string profilePicturePath);
    int? GetUserIdFromToken();
    Task<User?> GetCurrentUserAsync();
}

public class AuthService : IAuthService
{
    private readonly EfDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(EfDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

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
    public int? GetUserIdFromToken()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }
    
    public async Task<User?> GetCurrentUserAsync()
    {
        var userId = GetUserIdFromToken();
        if (userId.HasValue)
        {
            return await _context.Users.FindAsync(userId.Value);
        }

        return null;
    }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }
}