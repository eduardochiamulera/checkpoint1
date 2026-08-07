using Cursos.Models;
using Cursos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.Controllers;

/// <summary>Endpoints de autenticação e geração de token JWT.</summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    /// <summary>Registra um novo usuário com role Admin ou Instructor.</summary>
    /// <remarks>Role "Student" não é permitida neste endpoint. Use /register/student.</remarks>
    /// <response code="201">Usuário criado com sucesso.</response>
    /// <response code="400">Dados inválidos ou senha fraca.</response>
    /// <response code="409">Email já cadastrado.</response>
    [HttpPost("register")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _authService.RegisterAsync(request);
        return Created();
    }

    /// <summary>Autentica o usuário e retorna um token JWT.</summary>
    /// <remarks>Use o token retornado no botão **Authorize** do Swagger (formato: Bearer {token}).</remarks>
    /// <response code="200">Login realizado. Retorna AccessToken e ExpiresAt.</response>
    /// <response code="401">Credenciais inválidas ou conta bloqueada.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }
}
