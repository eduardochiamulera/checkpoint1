using Cursos.Domain.Interfaces;

namespace Cursos.Application.Enrollments.CompleteEnrollment;

public class CompleteEnrollmentHandler : IRequestHandler<CompleteEnrollmentCommand, EnrollmentDto>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CompleteEnrollmentHandler(
        IEnrollmentRepository enrollmentRepository,
        IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<EnrollmentDto> Handle(
        CompleteEnrollmentCommand request, 
        CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (enrollment is null)
            throw new KeyNotFoundException($"Enrollment with id {request.Id} not found");
        
        if (enrollment.Status != Domain.Entities.EnrollmentStatus.Active)
            throw new InvalidOperationException($"Only active enrollments can be completed. Current status: {enrollment.Status}");
        
        enrollment.Complete();
        
        await _enrollmentRepository.UpdateAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new EnrollmentDto(
            Id: enrollment.Id,
            StudentId: enrollment.StudentId,
            CourseId: enrollment.CourseId,
            Status: enrollment.Status.ToString(),
            EnrollmentDate: enrollment.EnrollmentDate,
            CompletionDate: enrollment.CompletionDate,
            CreatedAt: enrollment.CreatedAt);
    }
}
