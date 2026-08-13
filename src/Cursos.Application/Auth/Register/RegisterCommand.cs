using Cursos.Application.Auth.Login;
using Cursos.Application.Common;

namespace Cursos.Application.Auth.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string Name,
    string Phone
) : ICommand<AuthResultDto>;
