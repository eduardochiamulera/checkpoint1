using Cursos.Domain.Entities;
using Cursos.Domain.Interfaces;

namespace Cursos.Application.Students.CreateStudent;

public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, StudentDto>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateStudentHandler(
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<StudentDto> Handle(
        CreateStudentCommand request, 
        CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existing = await _studentRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException($"Student with email {request.Email} already exists");
        }
        
        var student = new Student(
            name: request.Name,
            email: request.Email,
            phone: request.Phone,
            birthDate: request.BirthDate);
        
        await _studentRepository.AddAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
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
