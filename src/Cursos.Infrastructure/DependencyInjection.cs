using System;
using Cursos.Domain.Interfaces;
using Cursos.Domain.Payments;
using Cursos.Infrastructure.Data;
using Cursos.Infrastructure.Gateways;
using Cursos.Infrastructure.Repositories;
using Cursos.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Cursos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0)),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
        
        // Repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Payment Gateway (Strategy Pattern)
        services.AddPaymentGateway(configuration);
        
        // Security Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator>(sp =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            return new JwtTokenGenerator(
                secretKey: jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured"),
                issuer: jwtSettings["Issuer"] ?? "CursosAPI",
                audience: jwtSettings["Audience"] ?? "CursosUsers",
                expirationMinutes: int.Parse(jwtSettings["ExpirationMinutes"] ?? "60")
            );
        });
        
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
