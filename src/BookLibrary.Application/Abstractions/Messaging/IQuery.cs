using BookLibrary.Domain.Abstractions;
using MediatR;

namespace BookLibrary.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
