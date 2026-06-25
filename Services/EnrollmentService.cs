using Cursos.Data;
using Cursos.Entites;
using Cursos.Exceptions;
using Cursos.Models;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Services;

public interface IEnrollmentService
{
    Task<EnrollmentResponse> EnrollAsync(int studentId, int courseId, string requestingUserId);
    Task<PaginatedResponse<EnrollmentResponse>> GetByStudentAsync(int studentId, EnrollmentPaginatedRequest request, string userId, bool isAdmin);
    Task<EnrollmentResponse> CancelAsync(int enrollmentId, string userId, bool isAdmin);
    Task<EnrollmentResponse> EnrollSelfAsync(int courseId, string userId);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly AppDbContext _appDbContext;

    public EnrollmentService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<EnrollmentResponse> EnrollAsync(int studentId, int courseId, string requestingUserId)
    {
        var course = await _appDbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == courseId && !x.IsDeleted)
            ?? throw new KeyNotFoundException("Curso não encontrado ou inativo.");

        var student = await _appDbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == studentId && !x.IsDeleted)
            ?? throw new KeyNotFoundException("Estudante não encontrado ou inativo.");

        var alreadyEnrolled = await _appDbContext.Enrollments
            .AnyAsync(x => x.StudentId == studentId && x.CourseId == courseId && x.Status == EnrollmentStatus.Ativo);

        if (alreadyEnrolled)
            throw new ConflictException("Estudante já matriculado neste curso.");

        var enrollment = new Enrollment
        {
            StudentId        = studentId,
            CourseId         = courseId,
            Status           = EnrollmentStatus.Ativo,
            DataMatricula    = DateTime.Now,
            UsuarioCriacao   = requestingUserId
        };

        await _appDbContext.Enrollments.AddAsync(enrollment);
        await _appDbContext.SaveChangesAsync();

        return new EnrollmentResponse(
            enrollment.Id,
            student.Id,
            student.NomeCompleto,
            course.Id,
            course.Titulo,
            enrollment.Status.ToString(),
            enrollment.DataMatricula,
            null
        );
    }

    public async Task<PaginatedResponse<EnrollmentResponse>> GetByStudentAsync(
        int studentId,
        EnrollmentPaginatedRequest request,
        string userId,
        bool isAdmin)
    {
        var student = await _appDbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == studentId && !x.IsDeleted)
            ?? throw new KeyNotFoundException("Estudante não encontrado.");

        if (!isAdmin && student.UserId != userId)
            throw new ForbiddenException("Acesso negado.");

        var query = _appDbContext.Enrollments
            .AsNoTracking()
            .Include(x => x.Course)
            .Include(x => x.Student)
            .Where(x => x.StudentId == studentId)
            .AsQueryable();

        var totalItems = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<EnrollmentStatus>(request.Status, out var statusEnum))
        {
            query = query.Where(x => x.Status == statusEnum);
        }

        var items = await query
            .OrderByDescending(x => x.DataMatricula)
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .Select(x => new EnrollmentResponse(
                x.Id,
                x.StudentId,
                x.Student.NomeCompleto,
                x.CourseId,
                x.Course.Titulo,
                x.Status.ToString(),
                x.DataMatricula,
                x.DataCancelamento
            ))
            .ToListAsync();

        return new PaginatedResponse<EnrollmentResponse>(items, request.Page, request.Size, totalItems);
    }

    public async Task<EnrollmentResponse> CancelAsync(int enrollmentId, string userId, bool isAdmin)
    {
        var enrollment = await _appDbContext.Enrollments
            .Include(x => x.Student)
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == enrollmentId)
            ?? throw new KeyNotFoundException("Matrícula não encontrada.");

        if (enrollment.Status == EnrollmentStatus.Cancelado)
            throw new InvalidOperationException("Matrícula já está cancelada.");

        if (!isAdmin && enrollment.Student.UserId != userId)
            throw new ForbiddenException("Acesso negado.");

        enrollment.Status           = EnrollmentStatus.Cancelado;
        enrollment.DataCancelamento = DateTime.Now;

        await _appDbContext.SaveChangesAsync();

        return new EnrollmentResponse(
            enrollment.Id,
            enrollment.StudentId,
            enrollment.Student.NomeCompleto,
            enrollment.CourseId,
            enrollment.Course.Titulo,
            enrollment.Status.ToString(),
            enrollment.DataMatricula,
            enrollment.DataCancelamento
        );
    }

    public async Task<EnrollmentResponse> EnrollSelfAsync(int courseId, string userId)
    {
        var student = await _appDbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted)
            ?? throw new KeyNotFoundException("Perfil de estudante não encontrado.");

        return await EnrollAsync(student.Id, courseId, userId);
    }
}