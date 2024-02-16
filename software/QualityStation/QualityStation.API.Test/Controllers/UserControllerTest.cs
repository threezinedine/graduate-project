using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using QualityStation.API.Controllers;
using QualityStation.API.Models;
using QualityStation.API.Services.PasswordService;
using QualityStation.API.Services.TokenService;
using QualityStation.API.Services.UserDbService;
using QualityStation.API.Services.UserValidationService;
using QualityStation.API.Test.Fixtures;
using QualityStation.API.Test.Utils;
using QualityStation.Shared.ModelDto.UserDto;
using System.Security.Claims;

namespace QualityStation.API.Test.Controllers
{
    public class UserControllerTest : IClassFixture<ControllerFixture>
    {
        private ControllerFixture m_fControllerFixture;

        // testing authentiation models
        private AuthenticationModel m_mFirstUserAuthenInfo;

        private string m_strFirstUserTestToken;
        private string m_strNonExistedUsername;
        private string m_strNonExistedId;

        private AuthenticationModel m_mFirstUserAuthenInfo_WithWrongUsername;
        private AuthenticationModel m_mFirstUserAuthenInfo_WithWrongPassword;
        private AuthenticationModel m_mUserAuthenInfo_WithTheSameUsernameWithTheFirstUser;
        private AuthenticationModel m_mUserAuthenInfo_WithUsernameHasInvalidCharacter;
        private AuthenticationModel m_mUserAuthenInfo_WithPasswordHasInvalidCharacter;

        public UserControllerTest(ControllerFixture controllerFixture)
        {
            m_fControllerFixture = controllerFixture;

            m_strFirstUserTestToken = "testing-token";
            m_strNonExistedUsername = "non-existed-Username";
            m_strNonExistedId = "non-existed-id";

            m_mFirstUserAuthenInfo = new AuthenticationModel
            {
                Username = "Username",
                Password = "password",
            };

            m_mFirstUserAuthenInfo_WithWrongUsername = new AuthenticationModel
            {
                Username = "wrong-Username",
                Password = "password",
            };

            m_mFirstUserAuthenInfo_WithWrongPassword = new AuthenticationModel
            {
                Username = "Username",
                Password = "password-wrong",
            };

            m_mUserAuthenInfo_WithTheSameUsernameWithTheFirstUser = new AuthenticationModel
            {
                Username = "Username",
                Password = "password-asdf",
            };

            m_mUserAuthenInfo_WithUsernameHasInvalidCharacter = new AuthenticationModel
            {
                Username = "Username$",
                Password = "adslkfajeh"
            };

            m_mUserAuthenInfo_WithPasswordHasInvalidCharacter = new AuthenticationModel
            {
                Username = "Username",
                Password = "adslkfajeh "
            };
        }
        [Fact]
        public async void GivenTheController_WhenAskAllUsers_ThenReturnsEmptyList()
        {
            // Arrange
            var cUserController = await m_fControllerFixture.InitUserController();

            // Act
            List<User> mUsers = await cUserController.GetAllUsers();

            // Assertion
            mUsers.Should().BeEmpty();
        }

        [Fact]
        public async void Post_RegisterTheUserWithUsernameAndPassword_ThenReturnsRegisteredUser()
        {
            // Arrange
            var cUserController = await m_fControllerFixture.InitUserController();

            // Act
            var response = await cUserController.RegisterUser(m_mFirstUserAuthenInfo);

            // Assertion
            var result = ResponseExtraction.GetObjectFromOkResponse<UserInfoDto>(response);

            result.Username.Should().Be(m_mFirstUserAuthenInfo.Username);
            result.Id.Should().NotBe(string.Empty);

            List<User> mUsers = await cUserController.GetAllUsers();

            mUsers.Should().NotBeEmpty()
                .And.HaveCount(1);
        }

        [Fact]
        public async void GivenAnAccountExists_WhenAddOtherAccountWithSameUsername_ReturnsConflictResponse()
        {
            // Arrange
            var cUserController = await m_fControllerFixture.InitUserController();
            await cUserController.RegisterUser(m_mFirstUserAuthenInfo);

            // Act
            var response = await cUserController.RegisterUser(
                m_mUserAuthenInfo_WithTheSameUsernameWithTheFirstUser);

            // Assert
            ResponseExtraction.GetErrorMessageFromConflictResponse(response!)
                    .Should().Be(UserControllerConstant.USER_EXISTED_ERROR_MESSAGE);
        }

        [Fact]
        public async void WhenRegisterAnAccountWhichUsernameHasInvalidCharacter_ThenReturnsBadRequestError()
        {
            // Arrange
            var cUserController = await m_fControllerFixture.InitUserController();

            // Act
            var response = await cUserController.RegisterUser(
                m_mUserAuthenInfo_WithUsernameHasInvalidCharacter);

            // Assert 
            ResponseExtraction.GetErrorMessageFromBadRequestResponse(response!)
                    .Should().Be(UserValidationServiceConstant.Username_CONTAINS_INVALID_CHARACTER);
        }

        [Fact]
        public async void WhenRegisterAnAccountWhichPasswordHasInvalidCharacter_ThenReturnsBadRequestError()
        {
            // Arrange
            var cUserController = await m_fControllerFixture.InitUserController();

            // Act
            var response = await cUserController.RegisterUser(
                m_mUserAuthenInfo_WithPasswordHasInvalidCharacter);

            // Assert 
            ResponseExtraction.GetErrorMessageFromBadRequestResponse(response!)
                    .Should().Be(UserValidationServiceConstant.PASSWORD_CONTAINS_INVALID_CHARACTER);
        }

        [Fact]
        public async void GivenAUserIsRegistered_WhenLogin_ThenReturnAToken()
        {
            // Arrange 
            var mocTokenService = new Mock<ITokenService>();
            mocTokenService.Setup(s => s.GenerateToken(It.IsAny<User>()))
                                .ReturnsAsync(m_strFirstUserTestToken);
            var cUserController = await m_fControllerFixture.InitUserController(mocTokenService.Object);
            await cUserController.RegisterUser(m_mFirstUserAuthenInfo);

            // Act
            var response = await cUserController.Login(m_mFirstUserAuthenInfo);

            // Assert
            string data = ResponseExtraction.GetObjectFromOkResponse<string>(response!);
            data.Should().Be(m_strFirstUserTestToken);
        }

        [Fact]
        public async void GivenAUserIsRegistered_WhenLoginWithWrongUsername_ThenReturnsNotFound()
        {
            // Arrange 
            var cUserController = await m_fControllerFixture.InitUserController();
            await cUserController.RegisterUser(m_mFirstUserAuthenInfo);

            // Act
            var response = await cUserController.Login(m_mFirstUserAuthenInfo_WithWrongUsername);

            // Assert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                        .Should().Be(UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

		[Fact]
		public async void GivenAUserIsRegistered_WhenLoginWithWrongPassword_ThenReturnsNotFound()
		{
			// Arrange 
			var cUserController = await m_fControllerFixture.InitUserController();
			await cUserController.RegisterUser(m_mFirstUserAuthenInfo);

			// Act
			var response = await cUserController.Login(m_mFirstUserAuthenInfo_WithWrongPassword);

			// Assert
            ResponseExtraction.GetErrorMessageFromBadRequestResponse(response!)
			            .Should().Be(UserControllerConstant.PASSWORD_IS_INCORRECT_ERROR_MESSAGE);
		}

        [Fact]
        public async void GivenAUserIsRegistered_WhenAskingTheUserFromValidToken_ThenReturnsTheUser()
        {
            // Arrange
            var mocTokenService = new Mock<ITokenService>();
            mocTokenService.Setup(s => s.GetUsernameFromToken(m_strFirstUserTestToken))
                            .ReturnsAsync(new TokenDecodeResult
                            {
                                Username = m_mFirstUserAuthenInfo.Username,
                            });
            var cUserController = await m_fControllerFixture.InitUserController(mocTokenService.Object);
            await cUserController.RegisterUser(m_mFirstUserAuthenInfo);

            // Act
            var response = await cUserController.GetUserByToken(m_strFirstUserTestToken);

            // Assert 
            var mUser = ResponseExtraction.GetObjectFromOkResponse<UserInfoDto>(response!);
        }

        [Fact]
        public async void GivenAUserIsRegistered_WhenAskingTheUserFromAnInvalidToken_ThenReturnsBadRequest()
        {
            // Arrage
            string strTokenErrorMessage = "Token is expired";

            var mocTokenService = new Mock<ITokenService>();
            mocTokenService.Setup(s => s.GetUsernameFromToken(m_strFirstUserTestToken))
                            .ReturnsAsync(new TokenDecodeResult
                            {
                                ErrorMessage = strTokenErrorMessage,
                            });

            var cUserController = await m_fControllerFixture.InitUserController(mocTokenService.Object);

            // Act
            var response = await cUserController.GetUserByToken(m_strFirstUserTestToken);

            // Assert
            var objectResult = response.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            ResponseExtraction.GetErrorMessageFromBadRequestResponse(response!)
                        .Should().Be(strTokenErrorMessage);
        }

        [Fact]
        public async void GivenAUserIsRegistered_WhenAskTheUserWithValidTokenButNonExistedUser_ThenReturnsBadRequest()
        {
            // Arrage
            var mocTokenService = new Mock<ITokenService>();
            mocTokenService.Setup(s => s.GetUsernameFromToken(m_strFirstUserTestToken))
                            .ReturnsAsync(new TokenDecodeResult
                            {
                                Username = m_strNonExistedUsername,
                            });

            var cUserController = await m_fControllerFixture.InitUserController(mocTokenService.Object);

            // Act
            var response = await cUserController.GetUserByToken(m_strFirstUserTestToken);

            // Assert
            ResponseExtraction.GetErrorMessageFromBadRequestResponse(response!)
                    .Should().Be(UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAUserIsLoggin_WhenGetTheDefaultRoute_ThenReturnThatUser()
        {
            // Arrage
            var cUserController = await m_fControllerFixture.InitUserController(
                                            loggedInUser: m_mFirstUserAuthenInfo);
            await cUserController.RegisterUser(m_mFirstUserAuthenInfo);

            // Act
            var respone = await cUserController.GetUser();

            // Assert
            var mUserInfo = ResponseExtraction.GetObjectFromOkResponse<UserInfoDto>(respone!);

            mUserInfo.Username.Should().Be(m_mFirstUserAuthenInfo.Username);
        }

        [Fact]
        public async void GivenAUserIsLoggedIn_WhenUpdateDataWithValidId_ThenThatEmailIsUpdated()
        {
            // Arrange
            string strTestedEmail = "threezinedine@gmail.com";
            var cUserController = await m_fControllerFixture
                                    .InitUserController(loggedInUser: m_mFirstUserAuthenInfo);
            await cUserController.RegisterUser(m_mFirstUserAuthenInfo);
            var mFirstUser = ResponseExtraction.GetObjectFromOkResponse<UserInfoDto>(
                                (await cUserController.GetUser())!);
            string strFirstUserId = mFirstUser.Id;
            UserInfoDto mNewUserData = new UserInfoDto
            {
                Id = strFirstUserId,
                Email = strTestedEmail,
            };

            // Act
            var response = await cUserController.UpdateUser(mNewUserData);

            // Assert
            var mResponseUser = ResponseExtraction.GetObjectFromOkResponse<UserInfoDto>(response!);
            mResponseUser.Username.Should().Be(mFirstUser.Username);
            mResponseUser.Email.Should().Be(strTestedEmail);

            var mRequestFirstUser = ResponseExtraction.GetObjectFromOkResponse<UserInfoDto>(
                                        (await cUserController.GetUser())!);
            mRequestFirstUser.Email.Should().Be(strTestedEmail);
        }
		[Fact]
		public async void GivenAUserIsLoggedIn_WhenUpdateDataWithNonExistedId_ThenThatEmailIsUpdated()
		{
			// Arrange
			string strTestedEmail = "threezinedine@gmail.com";
			var cUserController = await m_fControllerFixture
                                    .InitUserController(loggedInUser: m_mFirstUserAuthenInfo);
			await cUserController.RegisterUser(m_mFirstUserAuthenInfo);
			var mFirstUser = ResponseExtraction.GetObjectFromOkResponse<UserInfoDto>(       
                                                (await cUserController.GetUser())!);

			UserInfoDto mNewUserData = new UserInfoDto
			{
				Id = m_strNonExistedId,
				Email = strTestedEmail,
			};

			// Act
			var response = await cUserController.UpdateUser(mNewUserData);

			// Assert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                        .Should().Be(UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE);
		}
	}
}