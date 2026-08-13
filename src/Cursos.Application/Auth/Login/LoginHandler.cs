using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Cursos.Application.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
    
    public LoginHandler()
    {
    }
    
    public async Task<AuthResultDto> Handle(
        LoginCommand request, 
        CancellationToken cancellationToken)
    {
        // TODO: Implement actual authentication logic
        // This is a placeholder - should integrate with Identity or similar
        
        // _logger.LogInformation("Login attempt for email: {Email}", request.Email);
        
        // Simulated login - replace with actual auth logic
        var isValidCredentials = request.Email.Contains("@") && request.Password.Length >= 6;
        
        if (!isValidCredentials)
        {
            return new AuthResultDto(
                Success: false,
                Token: null,
                RefreshToken: null,
                ErrorMessage: "Invalid email or password",
                User: null);
        }
        
        var user = new UserDto(
            Id: Guid.NewGuid(),
            Email: request.Email,
            Name: request.Email.Split('@')[0],
            Roles: new List<string> { "User" });
        
        // Generate JWT token (placeholder - use actual JWT generation)
        var token = $"simulated_jwt_for_{request.Email}";
        var refreshToken = $"simulated_refresh_{Guid.NewGuid():N}";
        
        return new AuthResultDto(
            Success: true,
            Token: token,
            RefreshToken: refreshToken,
            ErrorMessage: null,
            User: user);
    }
}
