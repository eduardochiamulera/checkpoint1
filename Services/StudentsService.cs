using Cursos.Data;
using Cursos.Entites;
using Cursos.Exceptions;
using Cursos.Models;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Services;

public interface IStudentService
{
    Task<StudentResponse> CreateAsync(RegisterStudentRequest request, string userId);
    Task<PaginatedResponse<StudentResponse>> GetPaginatedAsync(StudentPaginatedRequest request);
    Task<StudentResponse> GetByIdAsync(int id, bool isAdmin, string userId);
    Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request, string userId, bool isAdmin);
    Task DeleteAsync(int id, string userId);
}

public class StudentService : IStudentService
{
    private readonly AppDbContext _appDbContext;
    private readonly IAuthService _authService;

    public StudentService(AppDbContext appDbContext, IAuthService authService)
    {
        _appDbContext = appDbContext;
        _authService = authService;
    }

    public async Task<StudentResponse> CreateAsync(RegisterStudentRequest request, string userId)
    {
        ArgumentNullException.ThrowIfNull(request);

        var createdUserId = await _authService.RegisterStudentAsync(request);

        var entity = new Student
        {
            Email          = request.Email,
            NomeCompleto   = request.NomeCompleto,
            UserId = createdUserId,
            DataCadastro = DateTime.Now,
            UsuarioCriacao = userId
        };

        await _appDbContext.Students.AddAsync(entity);
        await _appDbContext.SaveChangesAsync();

        return new StudentResponse(
            entity.Id,
            entity.NomeCompleto,
            entity.Email,
            new List<EnrollmentsResponse>()
        );
    }

    public async Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request, string userId, bool isAdmin)
    {
        ArgumentNullException.ThrowIfNull(request);

        await GetByIdAsync(id, isAdmin, userId);

        await _appDbContext.Students.Where(x => x.Id == id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(x => x.NomeCompleto, request.NomeCompleto)
                            .SetProperty(x => x.Email, request.Email)
                            .SetProperty(x => x.DataAlteracao, DateTime.Now)
                            .SetProperty(x => x.UsuarioAlteracao, userId));

        await _appDbContext.SaveChangesAsync();

        return await GetByIdAsync(id, isAdmin, userId);
    }

    public async Task<PaginatedResponse<StudentResponse>> GetPaginatedAsync(StudentPaginatedRequest request)
    {
        var query = _appDbContext.Students.Include(x => x.Enrollments).AsNoTracking().AsQueryable().Where(x => !x.IsDeleted);

        var totalItems = await query.CountAsync();

        if(!string.IsNullOrWhiteSpace(request.filter))
        {
            query = query.Where(x => x.NomeCompleto.Contains(request.filter));
        }

        if (!string.IsNullOrWhiteSpace(request.orderBy))
        {
            query = request.orderBy?.ToLower() switch
            {
                "nome"      => request.direction == "ASC"
                                    ? query.OrderBy(x => x.NomeCompleto)
                                    : query.OrderByDescending(x => x.NomeCompleto),
                "email" => request.direction == "ASC"
                                    ? query.OrderBy(x => x.Email)
                                    : query.OrderByDescending(x => x.Email),
                "datacadastro" => request.direction == "ASC"
                                    ? query.OrderBy(x => x.DataCadastro)
                                    : query.OrderByDescending(x => x.DataCadastro),
                _             => query.OrderBy(x => x.Id) // default
            };
        }

        var items = await query
            .Skip((request.page - 1) * request.size)
            .Take(request.size)
            .Select(x => 
                    new StudentResponse(
                        x.Id, 
                        x.NomeCompleto, 
                        x.Email, 
                        x.Enrollments.Select(y => new EnrollmentsResponse(y.Id, y.CourseId, y.StudentId, y.Status, y.DataMatricula)) 
                    ))
            .ToListAsync();

        return new PaginatedResponse<StudentResponse>(items, request.page, request.size, totalItems);
    }

    public async Task<StudentResponse> GetByIdAsync(int id, bool isAdmin, string userId)
    {
        if(id < 1)
        {
            throw new ArgumentException("Id inválido.");
        }

        var entity = await _appDbContext.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if( entity == null || entity.IsDeleted)
        {
            throw new KeyNotFoundException("Estudante não encontrado.");
        }

        if (!isAdmin && entity.UserId != userId)
        {
            throw new ForbiddenException("Acesso negado.");
        }

        return new StudentResponse(
            entity.Id,
            entity.NomeCompleto,
            entity.Email,
            entity.Enrollments?.Select(x => new EnrollmentsResponse(x.Id, x.CourseId, x.StudentId, x.Status, x.DataMatricula)) ?? new List<EnrollmentsResponse>()
        );
    }

    public async Task DeleteAsync(int id, string userId)
    {
        if(id < 1)
        {
            throw new ArgumentException("Id inválido.");
        }

         var exists = await _appDbContext.Students.AnyAsync(x => x.Id == id && !x.IsDeleted);

        if (!exists)
        {
            throw new KeyNotFoundException("Estudante não encontrado.");
        }

        await _appDbContext.Students.Where(x => x.Id == id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(x => x.IsDeleted, true)
                            .SetProperty(x => x.DataAlteracao, DateTime.Now)
                            .SetProperty(x => x.UsuarioAlteracao, userId));

        await _appDbContext.SaveChangesAsync();
    }
}