using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Commands.DeleteProduct;

internal sealed class DeleteProductByGuidCommandHandler : ICommandHandler<DeleteProductByGuidCommand>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductByGuidCommandHandler(IProductRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteProductByGuidCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.DeleteProductByGuidAsync(request.Guid, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, error: ex.Message);
        }
    }
}