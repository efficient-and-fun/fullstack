using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Common;
using WebApi.Model;

namespace WebApi;

public interface IUserService
{
    Task<UserResult> AddFriend(int userId, string newFriend);
}

public class UserService
{
    private readonly EfDbContext _context;

    public UserService(EfDbContext context)
    {
        _context = context;
    }

    public async Task<UserResult> AddFriend(int userId, string newFriend)
    {
        try
        {
            var existingFriendRequest =
                _context.FriendConnection.FirstOrDefault(fc => fc.UserId == userId && fc.Friend.UserName == newFriend);

            if (_context.Users.FirstOrDefault(u => u.UserId == userId) == null)
            {
                return new UserResult { Success = false, ErrorMessage = "User not found" };
            }

            if (existingFriendRequest == null)
            {
                var friendId = GetUserIdFromUserName(newFriend);
                if (friendId == -1)
                {
                    return new UserResult { Success = false, ErrorMessage = "User not found" };
                }

                _context.FriendConnection.Add(
                    new FriendConnection()
                    {
                        UserId = userId,
                        FriendId = friendId,
                        HasAcceptedFriendRequest = true
                        //TODO: this value needs to be adjust if the notifications are implemented
                    }
                );
            }
        }
        catch (Exception e)
        {
            return new UserResult{ Success = false, ErrorMessage = e.Message };
        }


        await _context.SaveChangesAsync();
        return new UserResult { Success = true };
    }


    public int GetUserIdFromUserName(string username)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == username);
        if (user == null)
        {
            return -1;
        }

        return user.UserId;
    }
}

public class UserResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}