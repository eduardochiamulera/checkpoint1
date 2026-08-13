using Cursos.Domain.Interfaces;

namespace Cursos.Application.Enrollments.GetEnrollmentsByStudent;

public class GetEnrollmentsByStudentHandler : IRequestHandler<GetEnrollmentsByStudentQuery, IEnumerable<EnrollmentDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    
    public GetEnrollmentsByStudentHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }
    
    public async Task<IEnumerable<EnrollmentDto>> Handle(
        GetEnrollmentsByStudentQuery request, 
        CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
        
        return enrollments.Select(e => new EnrollmentDto(
            Id: e.Id,
            StudentId: e.StudentId,
            CourseId: e.CourseId,
            Status: e.Status.ToString(),
            EnrollmentDate: e.EnrollmentDate,
            CompletionDate: e.CompletionDate,
            CreatedAt: e.CreatedAt));
    }
}
