using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Win32;
using QualityStation.Shared.ModelDto.UserDto;
using QualityStation.Shared.Pages.Services.StorageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.Pages.Providers
{
	public class CustomAuthenticationProvider : AuthenticationStateProvider
	{
		private readonly IStorageService m_serStorage;
		private readonly HttpClient m_httpClient;
        public CustomAuthenticationProvider(IStorageService storageService, HttpClient httpClient)
        {
			m_serStorage = storageService; 
			m_httpClient = httpClient;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var strToken = await m_serStorage.GetData<string>("token");
			var anonymous = new ClaimsIdentity();

			if (string.IsNullOrEmpty(strToken))
			{
				return new AuthenticationState(
						new ClaimsPrincipal(anonymous));
			}

			m_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", strToken);

			UserInfoDto? mUser = null;

			try
			{
                var result = await m_httpClient.GetAsync("users");

                if (!result.IsSuccessStatusCode)
                {
                    await m_serStorage.RemoveData<string>("token");
                    return new AuthenticationState(
                            new ClaimsPrincipal(anonymous));
                }
				
				mUser = await result.Content.ReadFromJsonAsync<UserInfoDto>();
			}
			catch (Exception ex)
			{
                await m_serStorage.RemoveData<string>("token");
                return new AuthenticationState(
                        new ClaimsPrincipal(anonymous));
			}

			if (mUser == null) 
			{
				return new AuthenticationState(
						new ClaimsPrincipal(anonymous));
			}

			return new AuthenticationState(
				new ClaimsPrincipal(
					new ClaimsIdentity(new List<Claim>
					{
						new Claim(ClaimTypes.NameIdentifier, mUser.Id),
						new Claim(ClaimTypes.Name, mUser.Username),
						new Claim(ClaimTypes.Role, mUser.Role),
					}, "JwtAuth"))); ;
		}
	}
}
