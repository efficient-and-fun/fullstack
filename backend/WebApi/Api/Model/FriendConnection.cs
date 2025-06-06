namespace WebApi.Api.Model;

/// <summary>
/// Represents a friendship connection between two users, including whether the request has been accepted.
/// </summary>
public class FriendConnection
{
    /// <summary>
    /// Unique identifier for the friend connection.
    /// </summary>
    public int FriendConnectionId { get; set; }

    /// <summary>
    /// The ID of the user who initiated the friend request.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation property for the user who initiated the friend request.
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// The ID of the user who is being added as a friend.
    /// </summary>
    public int FriendId { get; set; }

    /// <summary>
    /// Navigation property for the user who is being added as a friend.
    /// </summary>
    public User Friend { get; set; }

    /// <summary>
    /// Indicates whether the friend request has been accepted.
    /// </summary>
    public bool HasAcceptedFriendRequest { get; set; }
}