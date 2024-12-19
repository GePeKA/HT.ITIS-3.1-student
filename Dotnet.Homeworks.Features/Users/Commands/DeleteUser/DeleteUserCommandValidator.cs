using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandValidator: AbstractValidator<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(c => c.Guid)
            .MustAsync(UserMustExist).WithMessage("User does not exist");
    }

    private async Task<bool> UserMustExist(Guid userGuid, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByGuidAsync(userGuid, cancellationToken);

        return user != null;
    }
}
