using Cursos.Domain.Interfaces;
using System;
using System.Security.Cryptography;

namespace Cursos.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 10000;
    
    public string Hash(string password)
    {
        // Generate a random salt
        var salt = new byte[SaltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        
        // Hash the password with the salt
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(HashSize);
        
        // Combine salt and hash
        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);
        
        // Convert to base64
        return Convert.ToBase64String(hashBytes);
    }
    
    public bool Verify(string password, string passwordHash)
    {
        // Convert from base64
        var hashBytes = Convert.FromBase64String(passwordHash);
        
        // Extract salt and hash
        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);
        
        var storedHash = new byte[HashSize];
        Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);
        
        // Hash the provided password with the extracted salt
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var computedHash = pbkdf2.GetBytes(HashSize);
        
        // Compare the hashes
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}
