using Cursos.Application.Auth;
using Cursos.Application.Auth.AuthenticateUser;
using Cursos.Application.Auth.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Register a new user and return JWT token
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResultDto>> Register(
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
        {
            if (result.ErrorMessage?.Contains("already registered") == true)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Email already registered",
                    Detail = result.ErrorMessage,
                    Status = StatusCodes.Status409Conflict
                });
            }
            
            return BadRequest(new ProblemDetails
            {
                Title = "Registration failed",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        return CreatedAtAction(nameof(Login), new { }, result);
    }
    
    /// <summary>
    /// Login and return JWT token
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResultDto>> Login(
        [FromBody] AuthenticateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status401Unauthorized
            });
        }
        
        return Ok(result);
    }
    
    /// <summary>
    /// Test endpoint - requires authentication
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public ActionResult<UserDto> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
        
        return Ok(new UserDto(
            Id: Guid.Parse(userId!),
            Email: email!,
            Name: name!,
            Roles: roles));
    }
}
