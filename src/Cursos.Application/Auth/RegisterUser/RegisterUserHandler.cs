using Cursos.Application.Auth;
using Cursos.Domain.Entities;
using Cursos.Domain.Interfaces;
using MediatR;

namespace Cursos.Application.Auth.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    
    public RegisterUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<AuthResultDto> Handle(
        RegisterUserCommand request, 
        CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return new AuthResultDto(
                Success: false,
                Token: null,
                RefreshToken: null,
                ErrorMessage: "Email already registered",
                User: null);
        }
        
        // Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);
        
        // Create user
        var user = new User(
            email: request.Email,
            passwordHash: passwordHash,
            name: request.Name,
            phone: request.Phone);
        
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
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
