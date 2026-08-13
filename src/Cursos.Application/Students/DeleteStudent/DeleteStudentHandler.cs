using System.Threading;
using System.Threading.Tasks;
using Cursos.Domain.Interfaces;
using MediatR;

namespace Cursos.Application.Students.DeleteStudent;

public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, bool>
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

    public async Task<bool> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        await _studentRepository.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
