using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cursos.Data;
using Cursos.Entites;
using Cursos.Exceptions;
using Cursos.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Cursos.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<string> RegisterStudentAsync(RegisterStudentRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _appDbContext;
    private readonly IConfiguration _config;
    private SignInManager<IdentityUser> _signInManager;

    public AuthService(UserManager<IdentityUser> userManager, AppDbContext appDbContext, IConfiguration config, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _appDbContext = appDbContext;
        _config = config;
        _signInManager = signInManager;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        if(request.Role == "Student")
        {
            throw new InvalidOperationException("Role inválida.");
        }

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("Email já cadastrado.");

        var user = new IdentityUser
        {
            UserName = request.Email,
            Email    = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ArgumentException(errors);
        }

        // Valida se a role existe antes de atribuir
        var validRoles = new[] { "Admin", "Instructor", "Student" };
        if (!validRoles.Contains(request.Role))
            throw new ArgumentException($"Role inválida. Use: {string.Join(", ", validRoles)}");

        await _userManager.AddToRoleAsync(user, request.Role);

    }

    public async Task<string> RegisterStudentAsync(RegisterStudentRequest request)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        
        if (existing != null)
            throw new ConflictException("Email já cadastrado.");

        var user = new IdentityUser { UserName = request.Email, Email = request.Email };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
            throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Student");

        return user.Id;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            throw new UnauthorizedAccessException("Conta bloqueada. Tente novamente em 15 minutos.");

        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}