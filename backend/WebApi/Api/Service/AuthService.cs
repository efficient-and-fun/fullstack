using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApi.Api.Common;
using WebApi.Model;

namespace WebApi;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string username, string profilePicturePath);
}

public class AuthService : IAuthService
{
    private readonly EfDbContext _context;

    public AuthService(EfDbContext context)
    {
        _context = context;
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
        // Set a default profile picture path
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

        var token = GenerateJwtToken(email);

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
            Token = GenerateJwtToken(email)
        };
    }

    private string GenerateJwtToken(string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: new[] { new Claim(ClaimTypes.Name, email) },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public string ErrorMessage { get; set; }
}