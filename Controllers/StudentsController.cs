using System.Security.Claims;
using Cursos.Models;
using Cursos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.Controllers;

/// <summary>Gerenciamento de perfis de estudantes.</summary>
[ApiController]
[Route("api/v1/[controller]")]
public class StudentsController : BaseController
{

    private readonly ILogger<CoursesController> _logger;
    private readonly IStudentService _studentService;

    public StudentsController(ILogger<CoursesController> logger, IStudentService studentService)
    {
        _logger = logger;
        _studentService = studentService;
    }

     /// <summary>Cria um perfil de estudante vinculado a um usuário Identity.</summary>
    /// <remarks>Requer role **Admin**.</remarks>
    /// <response code="201">Estudante criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Sem permissão. Requer Admin.</response>
    /// <response code="409">Email já cadastrado.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(StudentResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(409)]
    public async Task<IResult> CreateAsync([FromBody]RegisterStudentRequest request)
    {
        var response = await _studentService.CreateAsync(request, GetUserid());
        return TypedResults.Created($"api/students/{response.id}", response);  
    }

      /// <summary>Lista estudantes com paginação e filtros.</summary>
    /// <remarks>Requer role **Admin**. Retorna dados sensíveis dos estudantes.</remarks>
    /// <response code="200">Lista paginada de estudantes.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Sem permissão. Requer Admin.</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PaginatedResponse<StudentResponse>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery]StudentPaginatedRequest request)
    {
        request ??= new StudentPaginatedRequest();

        var response = await _studentService.GetPaginatedAsync(request);

        return Ok(response);
    }

     /// <summary>Retorna os detalhes de um estudante pelo ID.</summary>
    /// <remarks>Acessível pelo **Admin** ou pelo **próprio estudante** (validação pelo UserId do token).</remarks>
    /// <response code="200">Detalhes do estudante.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Acesso negado. Estudante tentando ver dados de outro estudante.</response>
    /// <response code="404">Estudante não encontrado.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
    {
        var isAdmin = User.IsInRole("Admin");

        var response = await _studentService.GetByIdAsync(id, isAdmin, GetUserid());

        return Ok(response);
    }

     /// <summary>Atualiza o perfil de um estudante.</summary>
    /// <remarks>Acessível pelo **Admin** ou pelo **próprio estudante**. Atualiza também o email no Identity.</remarks>
    /// <response code="200">Estudante atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Acesso negado.</response>
    /// <response code="404">Estudante não encontrado.</response>
    /// <response code="409">Novo email já está em uso.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(StudentResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody]UpdateStudentRequest request)
    {
        var isAdmin = User.IsInRole("Admin");

        var response = await _studentService.UpdateAsync(id, request, GetUserid(), isAdmin);
        
        return Ok(response);  
    }

    /// <summary>Desativa um estudante (soft delete) e bloqueia o login no Identity.</summary>
    /// <remarks>Requer role **Admin**. O estudante não é removido fisicamente.</remarks>
    /// <response code="204">Estudante desativado com sucesso.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Sem permissão. Requer Admin.</response>
    /// <response code="404">Estudante não encontrado.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _studentService.DeleteAsync(id, GetUserid());

        return NoContent();
    }
}
