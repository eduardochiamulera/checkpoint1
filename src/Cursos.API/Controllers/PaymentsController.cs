using System;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Application.Payments.GetPaymentByEnrollment;
using Cursos.Application.Payments.ProcessPayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentResultDto>> ProcessPayment(
        [FromBody] ProcessPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Payment processing failed",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        return Ok(result);
    }
    
    [HttpGet("enrollment/{enrollmentId:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> GetPaymentByEnrollment(
        Guid enrollmentId,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentByEnrollmentQuery(enrollmentId);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result is null)
            return NotFound();
        
        return Ok(result);
    }
}
