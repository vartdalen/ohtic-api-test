using System.Security.Claims;

namespace Ohtic.Test.OAuth.Services.Abstractions
{
    public interface ITokenService
    {
        Task<ClaimsPrincipal> ValidateJwt(string jwt, bool validateLifetime = true);
        string WriteJwt(IEnumerable<Claim> claims, DateTimeOffset expires);
    }
}
