using System.Security.Claims;
using Cursos.Data;
using Cursos.Models;
using Cursos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.Controllers;

/// <summary>Gerenciamento de matrículas de estudantes em cursos.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
        => _enrollmentService = enrollmentService;

    /// <summary>Matricula o próprio estudante autenticado em um curso.</summary>
    /// <remarks>Requer role **Student**. O StudentId é resolvido automaticamente pelo token JWT.</remarks>
    /// <response code="201">Matrícula realizada com sucesso.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Sem permissão. Requer role Student.</response>
    /// <response code="404">Curso ou estudante não encontrado.</response>
    /// <response code="409">Estudante já matriculado neste curso.</response>
    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(EnrollmentResponse), 201)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> EnrollByAdmin([FromBody] EnrollmentAdminRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var response = await _enrollmentService.EnrollAsync(request.StudentId, request.CourseId, userId);
        return Created($"/api/enrollments/{response.Id}", response);
    }

    /// <summary>Admin matricula um estudante em um curso informando o StudentId.</summary>
    /// <remarks>Requer role **Admin**.</remarks>
    /// <response code="201">Matrícula realizada com sucesso.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Sem permissão. Requer Admin.</response>
    /// <response code="404">Curso ou estudante não encontrado.</response>
    /// <response code="409">Estudante já matriculado neste curso.</response>
    [HttpPost("admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(EnrollmentResponse), 201)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> EnrollSelf([FromBody] EnrollmentStudentRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Busca o studentId pelo UserId do token — não aceita studentId externo
        var enrollmentService = HttpContext.RequestServices
            .GetRequiredService<AppDbContext>();
        // Injeta AppDbContext direto no controller apenas para resolver o studentId
        // Alternativa: criar método no service

        var response = await _enrollmentService.EnrollSelfAsync(request.CourseId, userId);
        return Created($"/api/enrollments/{response.Id}", response);
    }

    /// <summary>Lista as matrículas de um estudante com paginação e filtro por status.</summary>
    /// <remarks>Acessível pelo **Admin** ou pelo **próprio estudante**.</remarks>
    /// <response code="200">Lista paginada de matrículas.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Acesso negado.</response>
    /// <response code="404">Estudante não encontrado.</response>
    [HttpGet("/api/v1/students/{studentId}/enrollments")]
    [ProducesResponseType(typeof(PaginatedResponse<EnrollmentResponse>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByStudent(
        [FromRoute] int studentId,
        [FromQuery] EnrollmentPaginatedRequest request)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var response = await _enrollmentService.GetByStudentAsync(studentId, request, userId, isAdmin);
        return Ok(response);
    }

    /// <summary>Cancela uma matrícula.</summary>
    /// <remarks>Acessível pelo **Admin** ou pelo **próprio estudante**. Matrícula já cancelada retorna 400.</remarks>
    /// <response code="200">Matrícula cancelada com sucesso.</response>
    /// <response code="400">Matrícula já está cancelada.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Acesso negado.</response>
    /// <response code="404">Matrícula não encontrada.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(EnrollmentResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Cancel([FromRoute] int id)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var response = await _enrollmentService.CancelAsync(id, userId, isAdmin);
        return Ok(response);
    }
}