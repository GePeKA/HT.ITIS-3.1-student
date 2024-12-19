using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.Dto;
using Dotnet.Homeworks.Infrastructure.Services;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : CqrsDecorator<CreateUserCommand, Result<CreateUserDto>>,
    ICommandHandler<CreateUserCommand, CreateUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRegistrationService _registrationService;

    public CreateUserCommandHandler(IPermissionCheck permissionCheck,
        IEnumerable<IValidator<CreateUserCommand>> validators,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IRegistrationService registrationService) 
        : base(permissionCheck, validators)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _registrationService = registrationService;
    }

    public override async Task<Result<CreateUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await base.Handle(request, cancellationToken);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            var newUser = new User()
            {
                Email = request.Email,
                Name = request.Name,
            };
            var insertedUserId = await _userRepository.InsertUserAsync(newUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _registrationService.RegisterAsync(new RegisterUserDto(request.Name, request.Email));

            return new Result<CreateUserDto>(new CreateUserDto(insertedUserId), true);
        }
        catch(Exception ex)
        {
            return new Result<CreateUserDto>(null, false, ex.Message);
        }
    }
}