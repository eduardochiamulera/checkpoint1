using System.Security.Claims;
using Cursos.Models;
using Cursos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{

    private readonly ILogger<CoursesController> _logger;
    private readonly ICourseService _courseService;

    public CoursesController(ILogger<CoursesController> logger, ICourseService courseService)
    {
        _logger = logger;
        _courseService = courseService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IResult> CreateAsync([FromBody]CourseRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _courseService.CreateCourseAsync(request, userId);
        return TypedResults.Created($"api/Courses/{response.id}", response);  
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery]CoursePaginatedRequest request)
    {
        request ??= new CoursePaginatedRequest();

        var response = await _courseService.GetPaginatedAsync(request);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
    {
        var response = await _courseService.GetByIdAsync(id);

        return Ok(response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody]CourseRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var response = await _courseService.UpdateCourseAsync(id, request, userId);
        
        return Ok(response);  
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _courseService.DeleteAsync(id);

        return NoContent();
    }
}
