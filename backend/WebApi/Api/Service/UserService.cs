using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Common;
using WebApi.Api.Model;

namespace WebApi;

/// <summary>
/// Defines user-related operations such as managing friendships.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Adds a friend to the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user who wants to add a friend.</param>
    /// <param name="newFriend">The username of the friend to be added.</param>
    /// <returns>
    /// A <see cref="UserResult"/> indicating success or failure of the operation.
    /// </returns>
    Task<UserResult> AddFriend(int userId, string newFriend);
    /// <summary>
    /// Removes a friend from the specified user's friend list.
    /// </summary>
    /// <param name="userId">The ID of the user who wants to remove a friend.</param>
    /// <param name="newFriend">The username of the friend to be removed.</param>
    /// <returns>
    /// A <see cref="UserResult"/> indicating success or failure of the operation.
    /// </returns>
    Task<UserResult> RemoveFriend(int userId, string newFriend);
}

/// <summary>
/// Provides user-related operations such as adding and removing friends.
/// </summary>
public class UserService : IUserService
{
    private readonly EfDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class with the given database context.
    /// </summary>
    /// <param name="context">Entity Framework database context.</param>
    public UserService(EfDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds a friend to the specified user's friend list.
    /// </summary>
    /// <param name="userId">The ID of the user who wants to add a friend.</param>
    /// <param name="newFriend">The username of the friend to be added.</param>
    /// <returns>
    /// A <see cref="UserResult"/> indicating success or failure and an optional error message.
    /// </returns>
    public async Task<UserResult> AddFriend(int userId, string newFriend)
    {
        try
        {
            if (_context.Users.FirstOrDefault(u => u.UserId == userId) == null)
            {
                return new UserResult { Success = false, ErrorMessage = "User not found" };
            }
            
            var existingFriendRequest = _context.FriendConnection.FirstOrDefault(fc => fc.UserId == userId && fc.Friend.UserName == newFriend);
            if (existingFriendRequest != null)
            {
                return new UserResult { Success = false, ErrorMessage = "Connection already exists" };
            }

            var friendId = GetUserIdFromUserName(newFriend);
            if (friendId == -1)
            {
                return new UserResult { Success = false, ErrorMessage = "Friend not found" };
            }
            
            if (userId == friendId)
            {
                return new UserResult { Success = false, ErrorMessage = "Find real friends" };
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

            await _context.SaveChangesAsync();
            return new UserResult { Success = true };
        }

        catch (Exception e)
        {
            
            Console.WriteLine($"An error occurred in AddFriend: {e}");
            
            return new UserResult { Success = false, ErrorMessage = "An unexpected error occurred. Please try again later." };
        }
    }

    /// <summary>
    /// Removes a friend from the specified user's friend list.
    /// </summary>
    /// <param name="userId">The ID of the user who wants to remove a friend.</param>
    /// <param name="newFriend">The username of the friend to be removed.</param>
    /// <returns>
    /// A <see cref="UserResult"/> indicating success or failure and an optional error message.
    /// </returns>
    public async Task<UserResult> RemoveFriend(int userId, string newFriend)
    {
        try
        {
            if (_context.Users.FirstOrDefault(u => u.UserId == userId) == null)
            {
                return new UserResult { Success = false, ErrorMessage = "User not found" };
            }
            
            var existingFriendRequest = _context.FriendConnection.FirstOrDefault(fc => fc.UserId == userId && fc.Friend.UserName == newFriend);
            if (existingFriendRequest == null)
            {
                return new UserResult { Success = false, ErrorMessage = "Connection does not exist" };
            }

            var friendId = GetUserIdFromUserName(newFriend);
            if (friendId == -1)
            {
                return new UserResult { Success = false, ErrorMessage = "Friend not found" };
            }
            
            if (userId == friendId)
            {
                return new UserResult { Success = false, ErrorMessage = "User cannot have themselves as friend" };
            }

            _context.FriendConnection.Remove(existingFriendRequest);
            
            await _context.SaveChangesAsync();
            return new UserResult { Success = true };
        }
        catch (Exception e)
        {
            return new UserResult { Success = false, ErrorMessage = e.Message };
        }
    }

    private int GetUserIdFromUserName(string username)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == username);
        if (user == null)
        {
            return -1;
        }

        return user.UserId;
    }
}

/// <summary>
/// Represents the result of a user-related operation, such as adding or removing a friend.
/// </summary>
public class UserResult
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// An optional error message describing why the operation failed; <c>null</c> if successful.
    /// </summary>
    public string? ErrorMessage { get; set; }
}