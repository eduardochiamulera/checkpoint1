using Cursos.Data;
using Cursos.Entites;
using Cursos.Models;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Services;

public interface ICourseService
{
    Task<CourseResponse> CreateCourseAsync(CourseRequest request);
    Task<PaginatedResponse<CourseResponse>> GetPaginatedAsync(CoursePaginatedRequest request);
    Task<CourseResponse> GetByIdAsync(int id);
    Task<CourseResponse> UpdateCourseAsync(int id, CourseRequest request);
    Task DeleteAsync(int id);
}

public class CourseService : ICourseService
{
    private readonly AppDbContext _appDbContext;

    public CourseService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<CourseResponse> CreateCourseAsync(CourseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = new Course
        {
            Titulo = request.titulo,
            Descricao = request.descricao,
            Categoria = request.categoria,
            CargaHoraria = request.cargaHoraria,
            DataCriacao = DateTime.Now,
            UsuarioCriacao = 1,
        };

        await _appDbContext.Courses.AddAsync(entity);
        await _appDbContext.SaveChangesAsync();

        return new CourseResponse(
            entity.Id,
            entity.Titulo,
            entity.Descricao,
            entity.Categoria,
            entity.CargaHoraria
        );
    }

    public async Task<CourseResponse> UpdateCourseAsync(int id, CourseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _appDbContext.Courses.Where(x => x.Id == id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(x => x.CargaHoraria, request.cargaHoraria)
                            .SetProperty(x => x.Categoria, request.categoria)
                            .SetProperty(x => x.Titulo, request.titulo)
                            .SetProperty(x => x.DataAlteracao, DateTime.Now)
                            .SetProperty(x => x.Descricao, request.descricao)
                            .SetProperty(x => x.UsuarioAlteracao, 1));

        await _appDbContext.SaveChangesAsync();

        return  await GetByIdAsync(id);
    }

    public async Task<PaginatedResponse<CourseResponse>> GetPaginatedAsync(CoursePaginatedRequest request)
    {
        var query = _appDbContext.Courses.AsQueryable().Where(x => !x.IsDeleted);

        var totalItems = await query.CountAsync();

        if (request.categories.Any())
            query = query.Where(x => request.categories.Contains(x.Categoria));

        if(!string.IsNullOrWhiteSpace(request.filter))
        {
            query = query.Where(x => x.Titulo.Contains(request.filter));
        }

        if (!string.IsNullOrWhiteSpace(request.orderBy))
        {
            if( request.direction == "ASC")
            {
                query = query.OrderBy(x => request.orderBy);
            }
            else
            {
                query = query.OrderByDescending(x => request.orderBy);
            }
        }

        var items = await query
            .Skip((request.page - 1) * request.size)
            .Take(request.size)
            .Select(x => new CourseResponse(x.Id, x.Titulo, x.Descricao, x.Categoria, x.CargaHoraria))
            .ToListAsync();

        return new PaginatedResponse<CourseResponse>(items, request.page, request.size, totalItems);
    }

    public async Task<CourseResponse> GetByIdAsync(int id)
    {
        if(id < 1)
        {
            throw new Exception("Id inválido");
        }

        var entity = await _appDbContext.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if( entity == null || entity.IsDeleted)
        {
            throw new Exception("Not Found");
        }

        return new CourseResponse(
            entity.Id,
            entity.Titulo,
            entity.Descricao,
            entity.Categoria,
            entity.CargaHoraria
        );
    }

    public async Task DeleteAsync(int id)
    {
        if(id < 1)
        {
            throw new Exception("Id inválido");
        }

        await _appDbContext.Courses.Where(x => x.Id == id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(x => x.IsDeleted, true)
                            .SetProperty(x => x.DataAlteracao, DateTime.Now)
                            .SetProperty(x => x.UsuarioAlteracao, 1));

        await _appDbContext.SaveChangesAsync();
    }
}