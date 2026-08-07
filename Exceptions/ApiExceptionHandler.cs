using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cursos.Domain.Exceptions;

namespace Cursos.Exceptions;

public sealed class ApiExceptionHandler
    : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(
        IProblemDetailsService problemDetails,
        ILogger<ApiExceptionHandler> logger)
    {
        _problemDetails = problemDetails;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, code, detail) = exception switch
        {
            DomainException domainException =>
                MapDomainException(domainException),

            KeyNotFoundException =>
                (StatusCodes.Status404NotFound,
                 "Recurso não encontrado",
                 "resource_not_found",
                 "O recurso solicitado não foi encontrado."),

            UnauthorizedAccessException =>
                (StatusCodes.Status403Forbidden,
                 "Acesso negado",
                 "forbidden",
                 "Você não possui permissão para acessar este recurso."),

            OperationCanceledException =>
                (StatusCodes.Status408RequestTimeout,
                 "Tempo esgotado",
                 "request_timeout",
                 "A operação excedeu o tempo limite."),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 "Erro interno",
                 "internal_error",
                 "Ocorreu um erro interno ao processar a solicitação.")
        };

        if (status >= 500)
        {
            _logger.LogError(
                exception,
                "Erro não tratado. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                "Erro da API. Code: {Code}, Status: {Status}, TraceId: {TraceId}",
                code,
                status,
                httpContext.TraceIdentifier);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{status}"
        };

        problem.Extensions["code"] = code;
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";

        await _problemDetails.WriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem
            });

        return true;
    }

    private static (
        int Status,
        string Title,
        string Code,
        string Detail) MapDomainException(
        DomainException exception)
    {
        return exception.Code switch
        {
            "payment_already_exists" or
            "active_payment_exists" or
            "idempotency_conflict" =>
                (StatusCodes.Status409Conflict,
                 "Conflito",
                 exception.Code,
                 exception.Message),

            "payment_declined" or
            "payment_gateway_failure" or
            "payment_gateway_timeout" or
            "invalid_payment_amount" or
            "invalid_currency" or
            "inactive_enrollment" =>
                (StatusCodes.Status422UnprocessableEntity,
                 "Regra de negócio não atendida",
                 exception.Code,
                 exception.Message),

            "invalid_enrollment" or
            "invalid_student" or
            "invalid_payment_method" or
            "missing_idempotency_key" =>
                (StatusCodes.Status400BadRequest,
                 "Entrada inválida",
                 exception.Code,
                 exception.Message),

            _ =>
                (StatusCodes.Status422UnprocessableEntity,
                 "Não foi possível processar a operação",
                 exception.Code,
                 exception.Message)
        };
    }
}