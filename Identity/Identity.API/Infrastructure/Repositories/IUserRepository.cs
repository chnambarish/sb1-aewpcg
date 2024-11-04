using Identity.API.Domain.Entities;

namespace Identity.API.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> UpdateLastLoginAsync(Guid userId);
}