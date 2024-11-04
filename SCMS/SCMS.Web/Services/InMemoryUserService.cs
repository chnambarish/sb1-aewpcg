using Microsoft.AspNetCore.Identity;

namespace SCMS.Web.Services;

public class InMemoryUserService : IUserService
{
    private readonly List<UserInfo> _users = new()
    {
        new UserInfo 
        { 
            Username = "user@example.com", 
            Password = "User123!", 
            Roles = new[] { "User" }
        },
        new UserInfo 
        { 
            Username = "admin@example.com", 
            Password = "Admin123!", 
            Roles = new[] { "Admin", "User" }
        }
    };

    public async Task<bool> ValidateUser(string username, string password)
    {
        var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        return user != null;
    }

    public async Task<string[]> GetUserRoles(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);
        return user?.Roles ?? Array.Empty<string>();
    }
}

public class UserInfo
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
}