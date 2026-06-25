using System.Security.Claims;
using Cursos.Data;
using Cursos.Models;
using Cursos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
        => _enrollmentService = enrollmentService;

    // Admin matricula qualquer estudante passando studentId no body
    [HttpPost("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EnrollByAdmin([FromBody] EnrollmentAdminRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var response = await _enrollmentService.EnrollAsync(request.StudentId, request.CourseId, userId);
        return Created($"/api/enrollments/{response.Id}", response);
    }

    // Student se matricula — studentId vem do token, não do body
    [HttpPost]
    [Authorize(Roles = "Student")]
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

    [HttpGet("/api/students/{studentId}/enrollments")]
    public async Task<IActionResult> GetByStudent(
        [FromRoute] int studentId,
        [FromQuery] EnrollmentPaginatedRequest request)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var response = await _enrollmentService.GetByStudentAsync(studentId, request, userId, isAdmin);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel([FromRoute] int id)
    {
        var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var response = await _enrollmentService.CancelAsync(id, userId, isAdmin);
        return Ok(response);
    }
}