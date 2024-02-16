using Microsoft.IdentityModel.Tokens;
using QualityStation.API.Models;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QualityStation.API.Services.TokenService
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration m_iConfiguration;
        public TokenService(IConfiguration configuration)
        {
			m_iConfiguration = configuration; 
        }
        public Task<string> GenerateToken(User mUser)
		{
			string strSecreteKey = m_iConfiguration.GetSection("Authentication:Jwt:SecreteKey").Value!;

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(strSecreteKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var claims = new[]
			{
				new Claim(ClaimTypes.Name, mUser.Username),
				new Claim(ClaimTypes.Role, mUser.Role),
			};

			var token = new JwtSecurityToken(
				claims: claims, 
				expires:  DateTime.UtcNow.AddHours(1),
				signingCredentials: credentials);

			return Task.FromResult(tokenHandler.WriteToken(token));
		}

		public Task<TokenDecodeResult> GetUsernameFromToken(string token)
		{
			var sDecodeResut = new TokenDecodeResult();
			var tokenHandler = new JwtSecurityTokenHandler();
			var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

			if (jsonToken == null)
			{
				sDecodeResut.ErrorMessage = TokenServiceConstant.INVALID_TOKEN_ERROR_MESSAGE;
			}
			else if (jsonToken.ValidTo < DateTime.UtcNow)
			{
				sDecodeResut.ErrorMessage = TokenServiceConstant.TOKEN_IS_EXPIRED;
			}
			else 
			{
				sDecodeResut.Username
					= jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
			}
			return Task.FromResult(sDecodeResut);
		}
	}
}
