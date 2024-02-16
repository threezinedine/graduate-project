using Microsoft.AspNetCore.Components.Authorization;
using QualityStation.Shared.ModelDto.UserDto;
using QualityStation.Shared.Pages.Services.AuthorizedHttpClientService;
using QualityStation.Shared.Pages.Services.StorageService;
using Radzen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.Pages.ViewModels
{
    public class ProfilePageViewModel : INotifyPropertyChanged
	{
        private UserInfoDto m_mUserInfo;
        private AuthorizedHttpClientService m_serAuthorizedHttpClient;
        private IStorageService m_serStorage;

		public string Username 
        {
            get
            {
                return m_mUserInfo.Username;
            }
        }

        public string Email
        {
            get
            {
                return m_mUserInfo.Email;
            }

            set
            {
                m_mUserInfo.Email = value;
                OnPropertyChanged();
            }
        }

        public ProfilePageViewModel(AuthorizedHttpClientService authorizedHttpClientService, 
                                    IStorageService storageService)
        {
            m_mUserInfo = new UserInfoDto();
            m_serAuthorizedHttpClient = authorizedHttpClientService;
            m_serStorage = storageService;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async Task LoadAuthenticatedUser()
        {
            await m_serAuthorizedHttpClient.AddToken();
            var response = await m_serAuthorizedHttpClient.HttpClient.GetAsync("users"); 
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserInfoDto>();
                if (result != null)
                {
                    m_mUserInfo = result;
                }
            }
        }

        public async Task UpdateUserProfile()
        {
            await m_serAuthorizedHttpClient.AddToken();
            var response = await m_serAuthorizedHttpClient.HttpClient
                                .PutAsJsonAsync<UserInfoDto>("users", m_mUserInfo);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserInfoDto>();

                if (result != null)
                {
                    m_mUserInfo = result;
                    OnPropertyChanged();
                }
            }
        }
    }
}
