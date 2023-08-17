namespace Ohtic.Test.OAuth
{
	internal static class InMemoryTokenStore
	{
		private static readonly Dictionary<string, string> RefreshTokens = new();

		internal static void AddRefreshToken(string userId, string refreshToken)
		{
			RefreshTokens[userId] = refreshToken;
		}

		internal static void RemoveRefreshToken(string userId)
		{
			RefreshTokens.Remove(userId);
		}

		internal static bool TryIdentifyRefreshToken(string refreshToken, out string? userId)
		{
			KeyValuePair<string, string>? kvp = RefreshTokens.FirstOrDefault(rt => rt.Value == refreshToken);
			userId = kvp?.Key;
			return userId != null;
		}
	}
}
