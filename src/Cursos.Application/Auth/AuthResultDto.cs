namespace Cursos.Application.Auth;

public record AuthResultDto(
    bool Success,
    string? Token,
    string? RefreshToken,
    string? ErrorMessage,
    UserDto? User
);

public record UserDto(
    Guid Id,
    string Email,
    string Name,
    List<string> Roles
);
