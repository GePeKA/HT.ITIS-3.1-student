using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

class CreateUserCommandValidator: AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email must not be empty!")
            .EmailAddress().WithMessage("Wrong email format")
            .MustAsync(IsUniqueEmailAsync).WithMessage("Email is not unique!");

        RuleFor(u => u.Name)
            .MinimumLength(5).WithMessage("Minimum length of Name is 5")
            .MaximumLength(20).WithMessage("Maximum length of Name is 20");
    }

    private async Task<bool> IsUniqueEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetUsersAsync(cancellationToken);
        return !users.Any(u => u.Email == email);
    }
}
