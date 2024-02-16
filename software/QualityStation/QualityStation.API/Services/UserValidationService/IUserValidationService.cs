using QualityStation.Shared.ModelDto.UserDto;

namespace QualityStation.API.Services.UserValidationService
{
	public static class UserValidationServiceConstant
	{
		public const string Username_CONTAINS_INVALID_CHARACTER = "Username contains invalid character.";
		public const string PASSWORD_CONTAINS_INVALID_CHARACTER = "Password contains invalid character.";
	}

	public interface IUserValidationService
	{
		public Task<bool> IsStringValid(string str);
		public Task<string?> IsAuthenticationModelValid(AuthenticationModel mAuthenticationModel);
		public Task<List<char>> GetInvalidChacters();
	}
}
