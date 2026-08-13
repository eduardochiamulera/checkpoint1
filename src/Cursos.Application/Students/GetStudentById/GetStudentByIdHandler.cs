using Cursos.Domain.Interfaces;

namespace Cursos.Application.Students.GetStudentById;

public class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, StudentDto?>
{
    private readonly IStudentRepository _studentRepository;
    
    public GetStudentByIdHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }
    
    public async Task<StudentDto?> Handle(
        GetStudentByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (student is null)
            return null;
        
        return new StudentDto(
            Id: student.Id,
            Name: student.Name,
            Email: student.Email,
            Phone: student.Phone,
            BirthDate: student.BirthDate,
            IsActive: student.IsActive,
            CreatedAt: student.CreatedAt);
    }
}
