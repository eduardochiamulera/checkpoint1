using Cursos.Application.Students.CreateStudent;
using Cursos.Application.Students.DeleteStudent;
using Cursos.Application.Students.GetAllStudents;
using Cursos.Application.Students.GetStudentById;
using Cursos.Application.Students.UpdateStudent;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllStudentsQuery(skip, take);
        var students = await _mediator.Send(query, cancellationToken);
        return Ok(students);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetStudentByIdQuery(id);
        var student = await _mediator.Send(query, cancellationToken);
        
        if (student is null)
            return NotFound();
        
        return Ok(student);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StudentDto>> Create(
        [FromBody] CreateStudentCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var student = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Student already exists",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> Update(
        Guid id,
        [FromBody] UpdateStudentCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "ID mismatch",
                Detail = "The ID in the URL does not match the ID in the body",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        try
        {
            var student = await _mediator.Send(command, cancellationToken);
            return Ok(student);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Student not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteStudentCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Student not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }
}
