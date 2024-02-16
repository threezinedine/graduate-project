using QualityStation.API.Models;

namespace QualityStation.API.Services.TokenService
{
	public class TokenServiceConstant
	{
		public const string INVALID_TOKEN_ERROR_MESSAGE = "Invalid token.";
		public const string TOKEN_IS_EXPIRED = "Token is expired.";
	}
    public class TokenDecodeResult
    {
        public string? Username { get; set; } 
        public string? ErrorMessage { get; set; }
    }

	public interface ITokenService
	{
		public Task<string> GenerateToken(User mUser);
		public Task<TokenDecodeResult> GetUsernameFromToken(string token);
	}
}
