using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Api.Model;

/// <summary>
/// Represents a user in the system, including authentication details, profile information,
/// dietary restrictions, and friend connections.
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    [Required, Key]
    public int UserId { get; set; }
    /// <summary>
    /// The username chosen by the user.
    /// </summary>
    public required string UserName { get; set; }
    /// <summary>
    /// The email address of the user.
    /// </summary>
    public required string Email { get; set; }
    /// <summary>
    /// The hashed passsword user for user authentication.
    /// </summary>
    public required string UserPassword { get; set; }
    /// <summary>
    /// The file path to the user's profile picture.
    /// </summary>
    public required string ProfilePicturePath { get; set; }
    /// <summary>
    /// Optional dietary restrictions provided by the user.
    /// </summary>
    public string? DietaryRestrictions { get; set; }
    /// <summary>
    /// List of users this user has added as friends.
    /// </summary>
    public ICollection<FriendConnection> Friends { get; set; }
    /// <summary>
    /// List of users who have added this user as a friend.
    /// </summary>
    public ICollection<FriendConnection> FriendOf { get; set; }
}

/// <summary>
/// Data Transfer Object (DTO) for exposing limited user information,
/// such as ID, username, email, and profile picture path.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    [Required, Key]
    public int UserId { get; set; }
    /// <summary>
    /// The username of the user.
    /// </summary>
    public required string UserName { get; set; }
    /// <summary>
    /// The email address of the user.
    /// </summary>
    public required string Email { get; set; }
    /// <summary>
    /// The file path to the user's profile picture.
    /// </summary>
    public required string ProfilePicturePath { get; set; }
}

/// <summary>
/// Represents the data required to register a new user, 
/// including credentials, profile picture, and AGB (terms) acceptance.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// The desired username of the new user.
    /// </summary>
    [Required]
    public required string Username { get; set; }
    /// <summary>
    /// The email address of the new user.
    /// </summary>
    [Required]
    public required string Email { get; set; }
    /// <summary>
    /// The password chosen by the user.
    /// </summary>
    [Required]
    public required string Password { get; set; }
    /// <summary>
    /// Confirmation of the password (must match the Password field).
    /// </summary>
    [Required]
    public required string Password2 { get; set; }
    /// <summary>
    /// The file path or URL to the user's profile picture.
    /// </summary>
    [Required]
    public required string ProfilePicturePath { get; set; }
    /// <summary>
    /// Indicates whether the user has accepted the terms and conditions (AGB).
    /// </summary>
    [Required]
    public required bool IsAGBAccepted { get; set; }
}

/// <summary>
/// Represents the data required to log in an existing user.
/// Uses the email address and the password.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Email address of the user that is used for the login.
    /// </summary>
    [Required]
    public required string Email { get; set; }
    /// <summary>
    /// Password of the user that is used for the login.
    /// </summary>
    [Required]
    public required string Password { get; set; }
}

/// <summary>
/// Represents the response containing a JWT token for authenticated users.
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// JWT Token used for the authentication.
    /// </summary>
    public required string Token { get; set; }
}

/// <summary>
/// Represents the response that is returned in case of an error in an API endpoint containing a message for further
/// information.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Message for further information about the error.
    /// </summary>
    public required string Message { get; set; }
}