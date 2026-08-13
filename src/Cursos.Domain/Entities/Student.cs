using System;
using Cursos.Domain.Common;

namespace Cursos.Domain.Entities;

public class Student : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public DateTime BirthDate { get; private set; }
    public bool IsActive { get; private set; }
    
    public Student(string name, string email, string phone, DateTime birthDate)
    {
        Name = name;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;
        IsActive = true;
    }
    
    public void Update(string name, string email, string phone, DateTime birthDate)
    {
        Name = name;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
