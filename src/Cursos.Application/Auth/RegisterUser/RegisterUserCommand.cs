using Cursos.Application.Auth;
using Cursos.Application.Common;

namespace Cursos.Application.Auth.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Password,
    string Name,
    string? Phone = null
) : ICommand<AuthResultDto>;
