using Cursos.Domain.Common;

namespace Cursos.Domain.Entities;

public class Course : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string Instructor { get; private set; }
    public int DurationHours { get; private set; }
    public bool IsActive { get; private set; }
    
    public Course(string name, string description, decimal price, string instructor, int durationHours)
    {
        Name = name;
        Description = description;
        Price = price;
        Instructor = instructor;
        DurationHours = durationHours;
        IsActive = true;
    }
    
    public void Update(string name, string description, decimal price, string instructor, int durationHours)
    {
        Name = name;
        Description = description;
        Price = price;
        Instructor = instructor;
        DurationHours = durationHours;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
