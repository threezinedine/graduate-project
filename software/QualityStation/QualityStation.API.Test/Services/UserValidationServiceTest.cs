using QualityStation.API.Services.UserValidationService;
using QualityStation.Shared.ModelDto.UserDto;
using QualityStation.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.API.Test.Services
{
	public class UserValidationServiceTest
	{
		private AuthenticationModel m_mValidAuthenticationModel;
		private AuthenticationModel m_mAuthenticationModelWithInvalidUsername;
		private AuthenticationModel m_mAuthenticationModelWithInvalidPassword;
		public UserValidationServiceTest()
		{
			m_mValidAuthenticationModel = new AuthenticationModel
			{
				Username = "Username",
				Password = "password",
			};

			m_mAuthenticationModelWithInvalidUsername = new AuthenticationModel
			{
				Username = "Username$",
				Password = "password",
			};

			m_mAuthenticationModelWithInvalidPassword = new AuthenticationModel
			{
				Username = "Username",
				Password = "password!",
			};
		}

		[Fact]
		public async void GivenAStringWhichIsValid_WhenCheckValidString_ThenReturnsTrue()
		{
			// Arrange
			IUserValidationService serUserValidationService = new UserValidationService();

			// Act
			bool bResult = await serUserValidationService.IsStringValid("Username");

			// Assert
			bResult.Should().BeTrue();
		}

		[Fact]
		public async Task GivenAStringWithInvalidChacters_WhenCheckValidString_ThenReturnsFalseAsync()
		{
			// Arrange
			IUserValidationService serUserValidationService = new UserValidationService();
			List<char> chInvalidCharacters = await serUserValidationService.GetInvalidChacters();
			string UsernameWithInvalidChacters 
					= $"Username{ListUtility.GetRandomValueFromAList<char>(chInvalidCharacters)}";

			// Act
			bool bResult = await serUserValidationService.IsStringValid(UsernameWithInvalidChacters);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public async Task GivenAValidAuthenticationModel_WhenAskModelIsValid_ThenReturnsTrueAsync()
		{
			// Arrage
			IUserValidationService serUserValidationService = new UserValidationService();

			// Act
			string? strError = await serUserValidationService.IsAuthenticationModelValid(
				m_mValidAuthenticationModel);

			// Assert
			strError.Should().BeNull();
		}

		[Fact]
		public async Task GivenAInvalidAuthenticationModelWhichHasInvalidUsername_WhenCheckValid_ThenReturnUsernameInvalidStringAsync()
		{
			// Arrage
			IUserValidationService serUserValidationService = new UserValidationService();

			// Act
			string? strError = await serUserValidationService.IsAuthenticationModelValid(
				m_mAuthenticationModelWithInvalidUsername);

			// Assert
			strError.Should().Be(UserValidationServiceConstant.Username_CONTAINS_INVALID_CHARACTER);
		}

		[Fact]
		public async Task GivenAnAuthenticationModelWithInvalidPassword_WhenCheckValid_ThenReturnsPasswordInvalidStringAsync()
		{
			// Arrage
			IUserValidationService serUserValidationService = new UserValidationService();

			// Act
			string? strError = await serUserValidationService.IsAuthenticationModelValid(
				m_mAuthenticationModelWithInvalidPassword);

			// Assert
			strError.Should().Be(UserValidationServiceConstant.PASSWORD_CONTAINS_INVALID_CHARACTER);
		}
	}
}
