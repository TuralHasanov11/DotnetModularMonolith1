using Ardalis.Result;
using MediatR;

namespace SharedKernel;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
