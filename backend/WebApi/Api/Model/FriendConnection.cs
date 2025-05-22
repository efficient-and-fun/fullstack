namespace WebApi.Api.Model;

public class FriendConnection
{
    public int FriendConnectionId { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int FriendId { get; set; }
    public User Friend { get; set; }

    public bool HasAcceptedFriendRequest { get; set; }
}