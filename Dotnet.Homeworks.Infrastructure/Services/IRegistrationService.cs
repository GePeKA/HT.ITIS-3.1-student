using Dotnet.Homeworks.Infrastructure.Dto;

namespace Dotnet.Homeworks.Infrastructure.Services;

public interface IRegistrationService
{
    public Task RegisterAsync(RegisterUserDto userDto);
}