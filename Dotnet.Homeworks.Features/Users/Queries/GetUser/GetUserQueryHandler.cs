using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Queries.GetUser;

public class GetUserQueryHandler : CqrsDecorator<GetUserQuery, Result<GetUserDto>>, IQueryHandler<GetUserQuery, GetUserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IPermissionCheck permissionCheck,
        IEnumerable<IValidator<GetUserQuery>> validators,
        IUserRepository userRepository) 
        : base(permissionCheck, validators)
    { 
        _userRepository = userRepository;
    }

    public override async Task<Result<GetUserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await base.Handle(request, cancellationToken);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            var user = await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken);

            return user == null
                ? new Result<GetUserDto>(null, false, "User with such ID does not exist")
                : new Result<GetUserDto>(new GetUserDto(user.Id, user.Name, user.Email), true);
        }
        catch (Exception ex)
        {
            return new Result<GetUserDto>(null, false, ex.Message);
        }
    }
}