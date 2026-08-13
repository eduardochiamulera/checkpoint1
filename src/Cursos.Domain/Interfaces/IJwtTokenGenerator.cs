using Cursos.Domain.Entities;

namespace Cursos.Domain.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
