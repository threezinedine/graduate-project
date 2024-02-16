using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QualityStation.API.Models;
using QualityStation.API.Services.PasswordService;
using QualityStation.API.Services.TokenService;
using QualityStation.API.Services.UserDbService;
using QualityStation.API.Services.UserValidationService;
using QualityStation.Shared.ModelDto.UserDto;

namespace QualityStation.API.Controllers
{
    public static class UserControllerConstant
    {
        public const string USER_DOES_NOT_EXIST_ERROR_MESSAGE = "User does not exist.";
        public const string PASSWORD_IS_INCORRECT_ERROR_MESSAGE = "Password is incorrect.";
        public const string USER_EXISTED_ERROR_MESSAGE = "User existed.";
        public const string USER_SERVICE_ERROR_MESSAGE = "User service error.";
    }

	[ApiController]
	[Route("users")]
	public class UserController : ControllerBase
	{
        private readonly IMapper m_iMapper;
        private readonly IUserValidationService m_serUserValidationService;
        private readonly ITokenService m_serTokenService;
        private readonly IPasswordService m_serPasswordService;
        private readonly IUserDbService m_serUserDbService;

        public UserController(IMapper mapper, 
                                IUserValidationService userValidationService,
                                ITokenService tokenService,
                                IPasswordService passwordService,
                                IUserDbService userDbService)
        {
            m_iMapper = mapper;
            m_serUserValidationService = userValidationService;
            m_serTokenService = tokenService;
            m_serPasswordService = passwordService;
            m_serUserDbService = userDbService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<UserInfoDto>> GetUser()
        {
            var mUser = await m_serUserDbService.GetUserByUsername(HttpContext.User?.Identity?.Name!);

            if (mUser == null) return BadRequest(UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE);

            return Ok(m_iMapper.Map<UserInfoDto>(mUser));
        }

        [HttpGet("get-all")]
        public Task<List<User>> GetAllUsers()
        {
            return m_serUserDbService.GetUsersAsync();
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserInfoDto?>> RegisterUser(AuthenticationModel authenticationModel)
        {
            string? strError = await m_serUserValidationService.IsAuthenticationModelValid(authenticationModel);

            if (strError != null)
            {
                return BadRequest(strError);
            }

            var mExistedUser = await m_serUserDbService.GetUserByUsername(authenticationModel.Username);

            if (mExistedUser != null)
            {
                return Conflict(UserControllerConstant.USER_EXISTED_ERROR_MESSAGE);
            }

            var mUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = authenticationModel.Username,
                EncryptedPassword = authenticationModel.Password 
            };

            var strErrorMessage = await m_serUserDbService.AddNewUser(mUser);

            if (strErrorMessage != null)
            {
                return BadRequest(strErrorMessage);
            }

            var mCreatedUser = await m_serUserDbService.GetUserByUsername(authenticationModel.Username);

            return Ok(m_iMapper.Map<UserInfoDto>(mCreatedUser));
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(AuthenticationModel mAuthenticationModel)
        {
            var mUser = await m_serUserDbService.GetUserByUsername(mAuthenticationModel.Username);

            if (mUser == null)
            {
                return NotFound(UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            if (!m_serPasswordService.Compare(mUser.EncryptedPassword, mAuthenticationModel.Password))
            {
                return BadRequest(UserControllerConstant.PASSWORD_IS_INCORRECT_ERROR_MESSAGE);
            }


            string strToken = await m_serTokenService.GenerateToken(mUser);

            return Ok(strToken);
        }

        [HttpPost]
        public async Task<ActionResult<UserInfoDto>> GetUserByToken(string token)
        {
            var sDecodeTokenResult = await m_serTokenService.GetUsernameFromToken(token);

            if (sDecodeTokenResult.Username == null) 
            {
                return BadRequest(sDecodeTokenResult.ErrorMessage);
            }

            var mUser = await m_serUserDbService.GetUserByUsername(sDecodeTokenResult.Username!);

            if (mUser == null)
            {
                return BadRequest(UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            return Ok(m_iMapper.Map<UserInfoDto>(mUser));
        }

        [HttpPut]
        public async Task<ActionResult<UserInfoDto>> UpdateUser(UserInfoDto mNewUserInfo)
        {
            var mUser = await m_serUserDbService.GetUserById(mNewUserInfo.Id);

            if (mUser == null)
            {
                return NotFound(UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            mUser.Email = mNewUserInfo.Email;

            var strErrorMessage = await m_serUserDbService.UpdateUser(mUser);

            if (strErrorMessage != null)
            {
                return BadRequest(UserControllerConstant.USER_SERVICE_ERROR_MESSAGE);
            }

            var mCreatedUser = await m_serUserDbService.GetUserById(mUser.Id);

            return Ok(m_iMapper.Map<UserInfoDto>(mCreatedUser));
        }
    }
}
