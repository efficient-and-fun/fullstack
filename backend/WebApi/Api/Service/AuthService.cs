using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApi.Api.Common;

namespace WebApi;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
}

public class AuthService : IAuthService
{
    private readonly EfDbContext _context;

    public AuthService(EfDbContext context)
    {
        _context = context;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.UserPassword == password);
        // TODO: hier noch die Hashfunktion einfügen

        if (foundUser == null)
        {
            return new AuthResult
            {
                Success = false
            };
        }

        return new AuthResult
        {
            Success = true,
            Token = GenerateJwtToken(email)
        };
    }

    private string GenerateJwtToken(string email)
    {
        // TODO: key und creds noch anpassen
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
}