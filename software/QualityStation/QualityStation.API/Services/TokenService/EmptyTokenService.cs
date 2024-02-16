using QualityStation.API.Models;

namespace QualityStation.API.Services.TokenService
{
	public class EmptyTokenService : ITokenService
	{
		public Task<string> GenerateToken(User mUser)
		{
			return Task.FromResult(string.Empty);
		}

		public Task<TokenDecodeResult> GetUsernameFromToken(string token)
		{
			return Task.FromResult(
				new TokenDecodeResult
				{
					Username = string.Empty
				});
		}
	}
}
