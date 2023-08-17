namespace Ohtic.Test.OAuth.Api.Models.Abstractions
{
    public interface ITokenResponse
    {
        string AccessToken { get; }
        string IdToken { get; }
        int ExpiresIn { get; }
    }
}