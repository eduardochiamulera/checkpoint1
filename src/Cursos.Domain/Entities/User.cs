using System;
using System.Collections.Generic;
using Cursos.Domain.Common;

namespace Cursos.Domain.Entities;

public class User : Entity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public List<string> Roles { get; private set; } = new();
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }
    
    public User(string email, string passwordHash, string name, string? phone = null)
    {
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
        Phone = phone;
        Roles = new List<string> { "User" }; // Default role
    }
    
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
    
    public void AddRole(string role)
    {
        if (!Roles.Contains(role))
            Roles.Add(role);
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
    
    public void Activate()
    {
        IsActive = true;
    }
}
