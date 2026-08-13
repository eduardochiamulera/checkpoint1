using Cursos.Application.Common;

namespace Cursos.Application.Auth.AuthenticateUser;

public record AuthenticateUserCommand(
    string Email,
    string Password
) : ICommand<AuthResultDto>;
