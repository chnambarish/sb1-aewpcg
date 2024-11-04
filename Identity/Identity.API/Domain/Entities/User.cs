namespace Identity.API.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public IReadOnlyCollection<string> Roles { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLogin { get; private set; }

    private User() { }

    public static User Create(string username, string passwordHash, IEnumerable<string> roles)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = passwordHash,
            Roles = roles.ToList().AsReadOnly(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
    }
}