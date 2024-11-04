using Identity.API.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.API.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<UserRepository> _logger;
    private const string CacheKeyPrefix = "User_";

    public UserRepository(IMemoryCache cache, ILogger<UserRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var cacheKey = $"{CacheKeyPrefix}{username}";
        return await Task.FromResult(_cache.Get<User>(cacheKey));
    }

    public async Task<bool> UpdateLastLoginAsync(Guid userId)
    {
        try
        {
            // In a real implementation, this would update the database
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last login for user {UserId}", userId);
            return false;
        }
    }
}