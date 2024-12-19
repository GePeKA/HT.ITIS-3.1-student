using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.UserManagement.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, GetAllUsersDto>, IHasPipeline
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetAllUsersDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userRepository.GetUsersAsync(cancellationToken);
            var userDtos = users.Select(u => new GetUserDto(u.Id, u.Name, u.Email));

            var getUsersDto = new GetAllUsersDto(userDtos);

            return new Result<GetAllUsersDto>(getUsersDto, true);
        }
        catch (Exception ex)
        {
            return new Result<GetAllUsersDto>(null, false, ex.Message);
        }
    }
}