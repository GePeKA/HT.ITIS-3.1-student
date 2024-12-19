using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator: AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email must not be empty!")
            .EmailAddress().WithMessage("This is not an email address")
            .Matches("^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(?:\\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@(?:[a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\\.)*(?:aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-z][a-z])$").WithMessage("Wrong email format")
            .MustAsync(IsUniqueEmailAsync).WithMessage("Email is not unique!");

        RuleFor(u => u.Name)
            .MinimumLength(2).WithMessage("Minimum length of Name is 2")
            .MaximumLength(20).WithMessage("Maximum length of Name is 20");
    }

    private async Task<bool> IsUniqueEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetUsersAsync(cancellationToken);
        return !users.Any(u => u.Email == email);
    }
}
