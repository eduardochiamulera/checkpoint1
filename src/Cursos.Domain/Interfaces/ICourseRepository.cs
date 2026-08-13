using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Domain.Entities;

namespace Cursos.Domain.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetAllAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<Course?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task AddAsync(Course course, CancellationToken cancellationToken = default);
    Task UpdateAsync(Course course, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
