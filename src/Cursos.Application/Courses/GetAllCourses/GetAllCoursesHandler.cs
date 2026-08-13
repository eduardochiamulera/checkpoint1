using Cursos.Domain.Interfaces;

namespace Cursos.Application.Courses.GetAllCourses;

public class GetAllCoursesHandler : IRequestHandler<GetAllCoursesQuery, IEnumerable<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    
    public GetAllCoursesHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    
    public async Task<IEnumerable<CourseDto>> Handle(
        GetAllCoursesQuery request, 
        CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetAllAsync(
            request.Skip, 
            request.Take, 
            cancellationToken);
        
        return courses.Select(c => new CourseDto(
            Id: c.Id,
            Name: c.Name,
            Description: c.Description,
            Price: c.Price,
            Instructor: c.Instructor,
            DurationHours: c.DurationHours,
            IsActive: c.IsActive,
            CreatedAt: c.CreatedAt));
    }
}
