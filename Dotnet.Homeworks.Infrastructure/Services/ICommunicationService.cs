using Dotnet.Homeworks.Shared.MessagingContracts.Email;

namespace Dotnet.Homeworks.Infrastructure.Services;

public interface ICommunicationService
{
    public Task SendEmailAsync(SendEmail sendEmailDto);
}