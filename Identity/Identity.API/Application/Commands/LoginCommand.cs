using MediatR;

namespace Identity.API.Application.Commands;

public record LoginCommand(string Username, string Password) : IRequest<LoginResult>;

public record LoginResult(bool Success, string? Token, string? Error = null);