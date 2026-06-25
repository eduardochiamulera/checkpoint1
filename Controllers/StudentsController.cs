using System.Security.Claims;
using Cursos.Models;
using Cursos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.Controllers;

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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> CreateAsync([FromBody]RegisterStudentRequest request)
    {
        var response = await _studentService.CreateAsync(request, GetUserid());
        return TypedResults.Created($"api/students/{response.id}", response);  
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery]StudentPaginatedRequest request)
    {
        request ??= new StudentPaginatedRequest();

        var response = await _studentService.GetPaginatedAsync(request);

        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin, Student")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
    {
        var isAdmin = User.IsInRole("Admin");

        var response = await _studentService.GetByIdAsync(id, isAdmin, GetUserid());

        return Ok(response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Student")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody]UpdateStudentRequest request)
    {
        var isAdmin = User.IsInRole("Admin");

        var response = await _studentService.UpdateAsync(id, request, GetUserid(), isAdmin);
        
        return Ok(response);  
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _studentService.DeleteAsync(id, GetUserid());

        return NoContent();
    }
}
