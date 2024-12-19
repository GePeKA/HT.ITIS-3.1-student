using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dotnet.Homeworks.Features.Users.PermissionCheck;

public class ClientPermissionCheck : IPermissionCheck<IClientRequest>
{
    private readonly HttpContext _httpContext;

    public ClientPermissionCheck(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public Task<IEnumerable<PermissionResult>> CheckPermissionAsync(IClientRequest request)
    {
        var claims = _httpContext.User.Claims;
        if (claims.Any(claim => claim.Type == ClaimTypes.NameIdentifier 
            && claim.Value == request.Guid.ToString()))
        {
            return Task.FromResult(new[] { new PermissionResult(true) }.AsEnumerable());
        }
        
        return Task.FromResult(new[] {new PermissionResult(false, "Can not access other user data!")}.AsEnumerable());
    }
}
