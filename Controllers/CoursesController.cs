using System.Security.Claims;
using Cursos.Models;
using Cursos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.Controllers;

/// <summary>Gerenciamento de cursos.</summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CoursesController : BaseController
{

    private readonly ILogger<CoursesController> _logger;
    private readonly ICourseService _courseService;

    public CoursesController(ILogger<CoursesController> logger, ICourseService courseService)
    {
        _logger = logger;
        _courseService = courseService;
    }

    
    /// <summary>Criar curso. Requer role Admin ou Instructor.</summary>
    /// <response code="201">Curso criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Sem permissão.</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(CourseResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IResult> CreateAsync([FromBody]CourseRequest request)
    {
        var response = await _courseService.CreateCourseAsync(request, GetUserid());
        return TypedResults.Created($"api/Courses/{response.id}", response);  
    }

    /// <summary>Lista cursos com paginação e filtros.</summary>
    /// <remarks>Endpoint público. Suporta filtro por texto e categoria, ordenação e paginação.</remarks>
    /// <response code="200">Lista paginada de cursos.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<CourseResponse>), 200)]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery]CoursePaginatedRequest request)
    {
        request ??= new CoursePaginatedRequest();

        var response = await _courseService.GetPaginatedAsync(request);

        return Ok(response);
    }

    /// <summary>Retorna os detalhes de um curso pelo ID.</summary>
    /// <response code="200">Detalhes do curso.</response>
    /// <response code="404">Curso não encontrado ou desativado.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CourseResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
    {
        var response = await _courseService.GetByIdAsync(id);

        return Ok(response);
    }

    /// <summary>Atualiza os dados de um curso.</summary>
    /// <remarks>Requer role **Admin** ou **Instructor**.</remarks>
    /// <response code="200">Curso atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Sem permissão.</response>
    /// <response code="404">Curso não encontrado.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(CourseResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody]CourseRequest request)
    {
        var response = await _courseService.UpdateCourseAsync(id, request, GetUserid());
        
        return Ok(response);  
    }

    /// <summary>Desativa um curso (soft delete).</summary>
    /// <remarks>Requer role **Admin**. O curso não é removido fisicamente do banco.</remarks>
    /// <response code="204">Curso desativado com sucesso.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Sem permissão. Requer Admin.</response>
    /// <response code="404">Curso não encontrado.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _courseService.DeleteAsync(id, GetUserid());

        return NoContent();
    }
}
