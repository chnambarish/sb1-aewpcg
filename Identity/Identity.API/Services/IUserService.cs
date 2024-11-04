namespace Identity.API.Services;

public interface IUserService
{
    Task<bool> ValidateUserAsync(string username, string password);
    Task<string[]> GetUserRolesAsync(string username);
    Task<UserInfo?> GetUserAsync(string username);
}

public class UserInfo
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
}