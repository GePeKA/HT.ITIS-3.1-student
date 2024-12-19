using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : CqrsDecorator<UpdateUserCommand, Result>,
    ICommandHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IPermissionCheck permissionCheck,
        IEnumerable<IValidator<UpdateUserCommand>> validators,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork) 
        : base(permissionCheck, validators)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await base.Handle(request, cancellationToken);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            await _userRepository.UpdateUserAsync(request.User, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, ex.Message);
        }
    }
}