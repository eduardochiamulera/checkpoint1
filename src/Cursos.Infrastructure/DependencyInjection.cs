using System;
using Cursos.Domain.Interfaces;
using Cursos.Domain.Payments;
using Cursos.Infrastructure.Data;
using Cursos.Infrastructure.Gateways;
using Cursos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cursos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
        
        // Repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Payment Gateway (Strategy Pattern)
        services.AddPaymentGateway(configuration);
        
        return services;
    }
    
    private static void AddPaymentGateway(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var gatewayType = configuration["PaymentGateway:Type"] ?? "Simulated";
        
        switch (gatewayType.ToLower())
        {
            case "simulated":
                services.AddSingleton<IPaymentGateway, SimulatedPaymentGateway>();
                break;
            case "stripe":
                // services.AddSingleton<IPaymentGateway, StripePaymentGateway>();
                break;
            case "paypal":
                // services.AddSingleton<IPaymentGateway, PayPalPaymentGateway>();
                break;
            default:
                throw new InvalidOperationException(
                    $"Unknown payment gateway type: {gatewayType}");
        }
    }
}
