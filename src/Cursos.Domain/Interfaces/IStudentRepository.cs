using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Domain.Entities;

namespace Cursos.Domain.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetAllAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(Student student, CancellationToken cancellationToken = default);
    Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
