using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cursos.Application.Students.CreateStudent;
using Cursos.Domain.Interfaces;
using MediatR;

namespace Cursos.Application.Students.UpdateStudent;

public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, StudentDto>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateStudentHandler(
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<StudentDto> Handle(
        UpdateStudentCommand request, 
        CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (student is null)
            throw new KeyNotFoundException($"Student with id {request.Id} not found");
        
        student.Update(request.Name, request.Email, request.Phone, request.BirthDate);
        
        await _studentRepository.UpdateAsync(student, cancellationToken);
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
