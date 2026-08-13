using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Domain.Entities;
using Cursos.Domain.Interfaces;
using MediatR;

namespace Cursos.Application.Enrollments.CreateEnrollment;

public class CreateEnrollmentHandler : IRequestHandler<CreateEnrollmentCommand, EnrollmentDto>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateEnrollmentHandler(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<EnrollmentDto> Handle(
        CreateEnrollmentCommand request, 
        CancellationToken cancellationToken)
    {
        // Validate student exists
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student is null)
            throw new KeyNotFoundException($"Student with id {request.StudentId} not found");
        
        // Validate course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
            throw new KeyNotFoundException($"Course with id {request.CourseId} not found");
        
        // Check if course is active
        if (!course.IsActive)
            throw new InvalidOperationException($"Course {course.Name} is not active");
        
        // Check if student is already enrolled
        var existingEnrollments = await _enrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
        var alreadyEnrolled = existingEnrollments.Any(e => e.CourseId == request.CourseId && e.Status == EnrollmentStatus.Active);
        
        if (alreadyEnrolled)
            throw new InvalidOperationException($"Student is already enrolled in this course");
        
        var enrollment = new Enrollment(request.StudentId, request.CourseId);
        
        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
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
