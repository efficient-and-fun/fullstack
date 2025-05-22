using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Api.Model;

namespace WebApi.Model;

[Table("Users")]
public class User
{
    [Required, Key]
    public int UserId { get; set; }
    
    public string UserName { get; set; }
    public string Email { get; set; }
    public string UserPassword { get; set; }
    public string ProfilePicturePath { get; set; }
    public string? DietaryRestrictions { get; set; }
    public ICollection<FriendConnection> Friends { get; set; }
    public ICollection<FriendConnection> FriendOf { get; set; }
}

public class UserDto
{
    [Required, Key]
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string ProfilePicturePath { get; set; }
}