using Ohtic.Test.OAuth.Api.Models;
using Refit;

namespace Ohtic.Test.OAuth.Api.Refit.Abstractions
{
	public interface IOAuthApi
    {
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        [Post("/token")]
        Task<TokenResponse> Token([Body(BodySerializationMethod.UrlEncoded)] TokenRequest request);
    }
}