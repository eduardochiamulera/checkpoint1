using System.Threading;
using System.Threading.Tasks;
using Cursos.Application.Auth.Login;
using Cursos.Application.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Http;
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
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResultDto>> Login(
        [FromBody] LoginCommand command,
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
    
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResultDto>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Registration failed",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        return CreatedAtAction(nameof(Login), new { }, result);
    }
}
