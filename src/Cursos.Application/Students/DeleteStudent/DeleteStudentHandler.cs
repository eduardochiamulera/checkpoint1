using Cursos.Domain.Interfaces;

namespace Cursos.Application.Students.DeleteStudent;

public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteStudentHandler(
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(
        DeleteStudentCommand request, 
        CancellationToken cancellationToken)
    {
        await _studentRepository.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
