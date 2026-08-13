using System;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Domain.Entities;
using Cursos.Domain.Interfaces;
using Cursos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
    
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }
    
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Set<User>().AddAsync(user, cancellationToken);
    }
    
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Set<User>().Update(user);
        await Task.CompletedTask;
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user is not null)
        {
            _context.Set<User>().Remove(user);
        }
    }
}
