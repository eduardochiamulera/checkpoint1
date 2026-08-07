using System.Security.Claims;
using Cursos.Models.Payments;
using Cursos.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.Controllers;

[ApiController]
[Route("api/v1/payments")]
[Authorize(Roles = "Student,Admin")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;
    private readonly PaymentOwnershipService _ownershipService;

    public PaymentsController(IPaymentService service, PaymentOwnershipService ownershipService)
    {
        _service = service;
        _ownershipService = ownershipService;
    }

    [HttpPost]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PaymentResponse>> CreateAsync(
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            ModelState.AddModelError(
                "Idempotency-Key",
                "O header Idempotency-Key é obrigatório.");

            return ValidationProblem(ModelState);
        }

        if (idempotencyKey.Length > 100)
        {
            ModelState.AddModelError(
                "Idempotency-Key",
                "O header Idempotency-Key deve possuir no máximo 100 caracteres.");

            return ValidationProblem(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

         var response = await _service.CreateAsync(
        request,
        userId,
        idempotencyKey,
        User.IsInRole("Admin"),
        cancellationToken);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = response.Id },
            response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        await _ownershipService.EnsureOwnerOrAdminAsync(id, userId, isAdmin, cancellationToken);

        var response = await _service.GetByIdAsync(
            id,
            userId,
            User.IsInRole("Admin"),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    [Authorize(Roles = "Student,Admin")]
    public async Task<ActionResult> ListAsync(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var payments = await _service.ListAsync(
            userId,
            User.IsInRole("Admin"),
            cancellationToken);

        // var response = payments.Select(payment => PaymentResponseMapper.ToResponse(payment))
        //     .ToList();

        return Ok();
    }
}