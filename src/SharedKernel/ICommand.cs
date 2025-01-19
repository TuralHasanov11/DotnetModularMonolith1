using Ardalis.Result;
using MediatR;

namespace SharedKernel;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
