using System.Security.Claims;

namespace AccountManager.Application.Auth
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey);
    }
}