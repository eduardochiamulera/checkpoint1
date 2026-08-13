using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Application.Students.CreateStudent;
using Cursos.Domain.Interfaces;
using MediatR;

namespace Cursos.Application.Students.GetAllStudents;

public class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, IEnumerable<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;
    
    public GetAllStudentsHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }
    
    public async Task<IEnumerable<StudentDto>> Handle(
        GetAllStudentsQuery request, 
        CancellationToken cancellationToken)
    {
        var students = await _studentRepository.GetAllAsync(
            request.Skip, 
            request.Take, 
            cancellationToken);
        
        return students.Select(s => new StudentDto(
            Id: s.Id,
            Name: s.Name,
            Email: s.Email,
            Phone: s.Phone,
            BirthDate: s.BirthDate,
            IsActive: s.IsActive,
            CreatedAt: s.CreatedAt));
    }
}
