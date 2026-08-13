using Cursos.Domain.Entities;
using Cursos.Domain.Interfaces;
using MediatR;

namespace Cursos.Application.Courses.CreateCourse;

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, CourseDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateCourseHandler(
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CourseDto> Handle(
        CreateCourseCommand request, 
        CancellationToken cancellationToken)
    {
        var course = new Course(
            name: request.Name,
            description: request.Description,
            price: request.Price,
            instructor: request.Instructor,
            durationHours: request.DurationHours);
        
        await _courseRepository.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new CourseDto(
            Id: course.Id,
            Name: course.Name,
            Description: course.Description,
            Price: course.Price,
            Instructor: course.Instructor,
            DurationHours: course.DurationHours,
            IsActive: course.IsActive,
            CreatedAt: course.CreatedAt);
    }
}
