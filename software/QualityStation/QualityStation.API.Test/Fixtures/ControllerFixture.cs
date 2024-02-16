using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QualityStation.API.Controllers;
using QualityStation.API.Helper;
using QualityStation.API.Services.DataParserService;
using QualityStation.API.Services.PasswordService;
using QualityStation.API.Services.StationDbService;
using QualityStation.API.Services.TokenService;
using QualityStation.API.Services.UserDbService;
using QualityStation.API.Services.UserValidationService;
using QualityStation.Shared.ModelDto.UserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.API.Test.Fixtures
{
    public class ControllerFixture : IDisposable
    {
        private IMapper Mapper;
        private IUserDbService m_serUserDbService;
        private IStationDbService m_serStationDbService;
        private AuthenticationModel m_mTestUser;
        private HttpContext m_httpContext;

        public ControllerFixture()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<APIMapper>();
            });

            Mapper = config.CreateMapper();
            m_serUserDbService = new TestUserDbService();
            m_mTestUser = new AuthenticationModel
            {
                Username = "Username",
                Password = "password",
            };

            m_httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, m_mTestUser.Username),
            })) };
        }

        public void Dispose()
        {

        }

        public async Task<UserController> InitUserController(ITokenService? serTokenService = null, 
                                                AuthenticationModel? loggedInUser = null, 
                                                bool bResetService = true)
        {
            if (serTokenService == null)
            {
                serTokenService = new EmptyTokenService();
            }

            if (bResetService)
            {
                m_serUserDbService = new TestUserDbService();
            }

            var cUserController = new UserController(Mapper,
                                                        new UserValidationService(),
                                                        serTokenService,
                                                        new NormalPasswordService(),
                                                        m_serUserDbService
                                                    );

            if (loggedInUser != null)
            {
                m_httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, loggedInUser.Username),
                })) };
                cUserController.ControllerContext = new ControllerContext { HttpContext = m_httpContext };
                await cUserController.RegisterUser(loggedInUser);
            }


            return cUserController;
        }

        public async Task<StationController> InitStationController(AuthenticationModel authenticationModel, 
                                                                    bool bResetService = true)
        {
            await InitUserController(loggedInUser: authenticationModel, bResetService: bResetService);

            if (bResetService)
            {
                m_serStationDbService = new TestStationDbService(m_serUserDbService);
            }

            var stationController = new StationController(Mapper,
                                                            m_serStationDbService,
                                                            m_serUserDbService,
                                                            new DataParserService());
            stationController.ControllerContext = new ControllerContext { HttpContext = m_httpContext };

            return stationController;
        }
    }
}
