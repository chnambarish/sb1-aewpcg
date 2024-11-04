using System.Security.Claims;

namespace Identity.API.Services;

public interface ITokenService
{
    string GenerateToken(IEnumerable<Claim> claims);
    bool ValidateToken(string token);
}