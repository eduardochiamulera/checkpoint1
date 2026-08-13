using MediatR;

namespace Cursos.Application.Common;

public interface ICommand<out TResponse> : IRequest<TResponse> { }
public interface ICommand : IRequest<Unit> { }
