using System.Security.Claims;
using Cursos.Models;
using Cursos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _authService.RegisterAsync(request);
        return Created();
    }

    [HttpPost("register/student")]
    public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentRequest request)
    {
        await _authService.RegisterStudentAsync(request);
        return Created();
    }

    [HttpPost("login")]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery]CoursePaginatedRequest request)
    {
        request ??= new CoursePaginatedRequest();

        var response = await _courseService.GetPaginatedAsync(request);

        return Ok(response);
    }
}
