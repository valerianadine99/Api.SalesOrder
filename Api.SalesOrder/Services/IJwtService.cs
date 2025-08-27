using System.Security.Claims;

namespace Api.SalesOrder.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(string username, string role, string userId);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
