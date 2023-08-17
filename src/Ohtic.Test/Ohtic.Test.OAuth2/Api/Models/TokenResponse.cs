using Ohtic.Test.OAuth.Api.Models.Abstractions;
using System.Text.Json.Serialization;

namespace Ohtic.Test.OAuth.Api.Models
{
	public sealed class TokenResponse : ITokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }

        [JsonPropertyName("scope")]
        public string Scope { get; init; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; init; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; init; }
    }
}