using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.UserManagement.Commands.DeleteUserByAdmin;

public class DeleteUserByAdminCommandHandler : ICommandHandler<DeleteUserByAdminCommand>, IHasPipeline
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserByAdminCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUserByAdminCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _userRepository.DeleteUserByGuidAsync(request.Guid, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, ex.Message);
        }
    }
}