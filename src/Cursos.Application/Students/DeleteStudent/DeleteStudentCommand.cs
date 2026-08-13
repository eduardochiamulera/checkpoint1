using Cursos.Application.Common;

namespace Cursos.Application.Students.DeleteStudent;

public record DeleteStudentCommand(Guid Id) : ICommand;
