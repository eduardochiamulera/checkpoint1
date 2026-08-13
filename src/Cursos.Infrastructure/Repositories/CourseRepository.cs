using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Domain.Entities;
using Cursos.Domain.Interfaces;
using Cursos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;
    
    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses.FindAsync(new object[] { id }, cancellationToken);
    }
    
    public async Task<IEnumerable<Course>> GetAllAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .OrderBy(c => c.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Course?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .FirstOrDefaultAsync(c => c.Name.ToLower() == slug.ToLower(), cancellationToken);
    }
    
    public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        await _context.Courses.AddAsync(course, cancellationToken);
    }
    
    public async Task UpdateAsync(Course course, CancellationToken cancellationToken = default)
    {
        _context.Courses.Update(course);
        await Task.CompletedTask;
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await GetByIdAsync(id, cancellationToken);
        if (course is not null)
        {
            _context.Courses.Remove(course);
        }
    }
}
