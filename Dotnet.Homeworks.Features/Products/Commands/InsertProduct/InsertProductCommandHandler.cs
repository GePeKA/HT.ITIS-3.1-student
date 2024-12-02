using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Commands.InsertProduct;

internal sealed class InsertProductCommandHandler : ICommandHandler<InsertProductCommand, InsertProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public InsertProductCommandHandler(IProductRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InsertProductDto>> Handle(InsertProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var newProduct = new Product() { Name = request.Name };

            var productGuid = await _repository.InsertProductAsync(newProduct, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Result<InsertProductDto>(
                new InsertProductDto(productGuid), 
                true
            );
        }
        catch (Exception ex)
        {
            return new Result<InsertProductDto>(val: null, false, error: ex.Message);
        }
    }
}