using MediatR;

namespace Cursos.Application.Common;

public interface IQuery<out TResponse> : IRequest<TResponse> { }
