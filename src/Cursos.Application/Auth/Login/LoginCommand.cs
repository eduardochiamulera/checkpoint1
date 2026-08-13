using Cursos.Application.Common;

namespace Cursos.Application.Auth.Login;

public record LoginCommand(
    string Email,
    string Password
) : ICommand<AuthResultDto>;
