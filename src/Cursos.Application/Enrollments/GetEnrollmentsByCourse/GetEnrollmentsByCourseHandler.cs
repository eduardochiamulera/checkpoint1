using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Application.Enrollments.CreateEnrollment;
using Cursos.Domain.Interfaces;
using MediatR;

namespace Cursos.Application.Enrollments.GetEnrollmentsByCourse;

public class GetEnrollmentsByCourseHandler : IRequestHandler<GetEnrollmentsByCourseQuery, IEnumerable<EnrollmentDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    
    public GetEnrollmentsByCourseHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }
    
    public async Task<IEnumerable<EnrollmentDto>> Handle(
        GetEnrollmentsByCourseQuery request, 
        CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        
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
