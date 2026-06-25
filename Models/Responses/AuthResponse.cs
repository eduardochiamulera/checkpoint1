namespace Cursos.Models;

/// <summary>Resposta de autenticação com o token JWT.</summary>
public record AuthResponse(
    /// <summary>Token JWT para uso no header Authorization: Bearer {token}.</summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    string AccessToken,
    /// <summary>Data e hora de expiração do token (UTC).</summary>
    /// <example>2026-06-26T00:00:00Z</example>
    DateTime ExpiresAt
);