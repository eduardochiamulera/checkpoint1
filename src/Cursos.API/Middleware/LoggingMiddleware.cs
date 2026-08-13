using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cursos.API.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    
    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        var userId = GetUserId(context);
        var startTime = Stopwatch.GetTimestamp();
        
        try
        {
            _logger.LogInformation(
                "[{CorrelationId}] {Method} {Path} started - User: {UserId}",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                userId);
            
            await _next(context);
            
            var elapsedMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
            _logger.LogInformation(
                "[{CorrelationId}] {Method} {Path} completed with status {StatusCode} in {ElapsedMs}ms - User: {UserId}",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                elapsedMs,
                userId);
        }
        catch (Exception ex)
        {
            var elapsedMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
            _logger.LogError(
                ex,
                "[{CorrelationId}] {Method} {Path} failed with error after {ElapsedMs}ms - User: {UserId}",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                elapsedMs,
                userId);
            
            throw;
        }
    }
    
    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            return correlationId.ToString();
        }
        
        correlationId = Guid.NewGuid().ToString("N")[..8];
        context.Response.Headers["X-Correlation-ID"] = correlationId;
        return correlationId;
    }
    
    private static string? GetUserId(HttpContext context)
    {
        return context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
    }
}
