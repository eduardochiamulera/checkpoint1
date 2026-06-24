using Cursos.Data;
using Cursos.Entites;
using Cursos.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task RegisterStudentAsync(RegisterStudentRequest request);
}

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _appDbContext;

    public AuthService(UserManager<IdentityUser> userManager, AppDbContext appDbContext)
    {
        _userManager   = userManager;
        _appDbContext  = appDbContext;
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

    public async Task RegisterStudentAsync(RegisterStudentRequest request)
{
    var existing = await _userManager.FindByEmailAsync(request.Email);
    if (existing != null)
        throw new InvalidOperationException("Email já cadastrado.");

    var user = new IdentityUser { UserName = request.Email, Email = request.Email };

    var result = await _userManager.CreateAsync(user, request.Password);
    if (!result.Succeeded)
        throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));

    await _userManager.AddToRoleAsync(user, "Student");

    var student = new Student
    {
        Email          = request.Email,
        NomeCompleto   = request.NomeCompleto,
        UserId = user.Id
    };

    await _appDbContext.Students.AddAsync(student);
    await _appDbContext.SaveChangesAsync();
}
}