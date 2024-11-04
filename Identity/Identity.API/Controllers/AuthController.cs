using MediatR;
using Microsoft.AspNetCore.Mvc;
using Identity.API.Application.Commands;
using Identity.API.Models;
using FluentValidation;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<LoginRequest> _validator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IMediator mediator,
        IValidator<LoginRequest> validator,
        ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _validator = validator;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = new LoginCommand(request.Username, request.Password);
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(new LoginResponse { Token = result.Token! });
    }
}