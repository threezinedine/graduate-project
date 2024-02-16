
using QualityStation.Shared.ModelDto.UserDto;

namespace QualityStation.API.Services.UserValidationService
{
	public class UserValidationService : IUserValidationService
	{
		private readonly List<char> m_chInvalidCharacters = new List<char> { '$', ' ', '!', '#'  };
		public Task<List<char>> GetInvalidChacters()
		{
			return Task.FromResult(m_chInvalidCharacters);
		}

		public async Task<string?> IsAuthenticationModelValid(AuthenticationModel mAuthenticationModel)
		{
			string? strError = null;

			if (!await IsStringValid(mAuthenticationModel.Username))
			{
				strError = UserValidationServiceConstant.Username_CONTAINS_INVALID_CHARACTER;
			}
			else if (!await IsStringValid(mAuthenticationModel.Password))
			{
				strError = UserValidationServiceConstant.PASSWORD_CONTAINS_INVALID_CHARACTER;
			}
			return strError;
		}

		public Task<bool> IsStringValid(string str)
		{
			return Task.FromResult(!str.Any(c => m_chInvalidCharacters.Contains(c)));
		}
	}
}
