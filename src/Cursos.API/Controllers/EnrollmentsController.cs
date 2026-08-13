using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Application.Enrollments.CancelEnrollment;
using Cursos.Application.Enrollments.CompleteEnrollment;
using Cursos.Application.Enrollments.CreateEnrollment;
using Cursos.Application.Enrollments.GetEnrollmentsByCourse;
using Cursos.Application.Enrollments.GetEnrollmentsByStudent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<EnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetByStudent(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var query = new GetEnrollmentsByStudentQuery(studentId);
        var enrollments = await _mediator.Send(query, cancellationToken);
        return Ok(enrollments);
    }
    
    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<EnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetByCourse(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var query = new GetEnrollmentsByCourseQuery(courseId);
        var enrollments = await _mediator.Send(query, cancellationToken);
        return Ok(enrollments);
    }
    
    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EnrollmentDto>> Create(
        [FromBody] CreateEnrollmentCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetByStudent), new { studentId = enrollment.StudentId }, enrollment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Enrollment conflict",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }
    
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EnrollmentDto>> Complete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CompleteEnrollmentCommand(id);
            var enrollment = await _mediator.Send(command, cancellationToken);
            return Ok(enrollment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Enrollment not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid operation",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
    
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EnrollmentDto>> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CancelEnrollmentCommand(id);
            var enrollment = await _mediator.Send(command, cancellationToken);
            return Ok(enrollment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Enrollment not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid operation",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
