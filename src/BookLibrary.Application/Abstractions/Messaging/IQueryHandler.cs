﻿using BookLibrary.Domain.Abstractions;
using MediatR;

namespace BookLibrary.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
