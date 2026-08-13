using Cursos.Application.Courses.CreateCourse;
using Cursos.Application.Courses.GetAllCourses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCoursesQuery(skip, take);
        var courses = await _mediator.Send(query, cancellationToken);
        return Ok(courses);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var course = await _mediator.Send(new GetAllCoursesQuery().WithId(id), cancellationToken);
        // TODO: Implement GetCourseById query
        return NotFound();
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseDto>> Create(
        [FromBody] CreateCourseCommand command,
        CancellationToken cancellationToken)
    {
        var course = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
    }
}

// Extension method for query
public static class GetAllCoursesQueryExtensions
{
    public static GetAllCoursesQuery WithId(this GetAllCoursesQuery query, Guid id)
        => query; // Placeholder - need separate GetCourseByIdQuery
}
