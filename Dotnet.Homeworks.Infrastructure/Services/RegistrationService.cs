using Dotnet.Homeworks.Infrastructure.Dto;
using Dotnet.Homeworks.Shared.MessagingContracts.Email;

namespace Dotnet.Homeworks.Infrastructure.Services;

public class RegistrationService : IRegistrationService
{
    private readonly ICommunicationService _communicationService;

    public RegistrationService(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    public async Task RegisterAsync(RegisterUserDto userDto)
    {
        await Task.Delay(100);

        await _communicationService.SendEmailAsync(new SendEmail(
            userDto.Name,
            userDto.Email,
            "Регистрация",
            "Вы успешно прошли регистрацию"));
    }
}