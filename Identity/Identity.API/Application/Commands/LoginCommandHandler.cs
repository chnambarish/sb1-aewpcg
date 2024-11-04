using MediatR;
using Identity.API.Services;
using Microsoft.Extensions.Logging;

namespace Identity.API.Application.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserService userService,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.ValidateUserAsync(request.Username, request.Password);
            if (user == null)
            {
                _logger.LogWarning("Login failed for user {Username}", request.Username);
                return new LoginResult(false, null, "Invalid credentials");
            }

            var token = _tokenService.GenerateToken(user);
            _logger.LogInformation("User {Username} logged in successfully", request.Username);
            return new LoginResult(true, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", request.Username);
            return new LoginResult(false, null, "An error occurred during login");
        }
    }
}