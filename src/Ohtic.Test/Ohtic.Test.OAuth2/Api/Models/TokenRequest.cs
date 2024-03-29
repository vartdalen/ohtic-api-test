namespace Ohtic.Test.OAuth.Api.Models
{
	public sealed class TokenRequest
    {
		public string grant_type { get; set; }
		public string? client_id { get; set; }
		public string? client_secret { get; set; }
		public string? code_verifier { get; set; }
		public string? code { get; set; }
		public string? refresh_token { get; set; }
		public string? redirect_uri { get; set; }
		public string? scope { get; set; }
    }
}