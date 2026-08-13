using Cursos.Application;
using Cursos.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure (EF Core, Repositories, Gateways)
builder.Services.AddInfrastructure(builder.Configuration);

// Application (MediatR, Use Cases)
builder.Services.AddApplication();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseExceptionHandler();
app.MapControllers();

app.Run();

// Global Exception Handler
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred");
        
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error",
            Detail = "An unexpected error occurred. Please try again later.",
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };
        
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}
