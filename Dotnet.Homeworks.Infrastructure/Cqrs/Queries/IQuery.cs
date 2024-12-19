using Dotnet.Homeworks.Shared.Dto;
using Dotnet.Homeworks.Mediator;

namespace Dotnet.Homeworks.Infrastructure.Cqrs.Queries;

public interface IQuery<TResponse>: IRequest<Result<TResponse>>
{
}