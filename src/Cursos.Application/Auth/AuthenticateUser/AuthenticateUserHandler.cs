using Cursos.Domain.Interfaces;

namespace Cursos.Application.Auth.AuthenticateUser;

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    
    public AuthenticateUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    
    public async Task<AuthResultDto> Handle(
        AuthenticateUserCommand request, 
        CancellationToken cancellationToken)
    {
        // Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (user is null)
        {
            return new AuthResultDto(
                Success: false,
                Token: null,
                RefreshToken: null,
                ErrorMessage: "Invalid email or password",
                User: null);
        }
        
        // Verify password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResultDto(
                Success: false,
                Token: null,
                RefreshToken: null,
                ErrorMessage: "Invalid email or password",
                User: null);
        }
        
        // Check if user is active
        if (!user.IsActive)
        {
            return new AuthResultDto(
                Success: false,
                Token: null,
                RefreshToken: null,
                ErrorMessage: "User account is deactivated",
                User: null);
        }
        
        // Update last login
        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);
        
        // Generate tokens
        var token = _jwtTokenGenerator.GenerateToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        
        return new AuthResultDto(
            Success: true,
            Token: token,
            RefreshToken: refreshToken,
            ErrorMessage: null,
            User: new UserDto(
                Id: user.Id,
                Email: user.Email,
                Name: user.Name,
                Roles: user.Roles));
    }
}
