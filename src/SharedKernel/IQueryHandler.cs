using Ardalis.Result;
using MediatR;

namespace SharedKernel;

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
