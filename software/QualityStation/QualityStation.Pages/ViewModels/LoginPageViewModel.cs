using QualityStation.Shared.ModelDto.UserDto;
using QualityStation.Shared.Pages.Services.StorageService;
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
    public class LoginResult
    {
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
    
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        private string m_strUsername = string.Empty;
        private string m_strPassword = string.Empty;

        private readonly HttpClient m_hClient;
        private readonly IStorageService m_serStorage;

        public LoginPageViewModel(HttpClient httpClient, 
                                    IStorageService storageService)
        {
            m_hClient = httpClient; 
            m_serStorage = storageService;
        }

        public string Username
        {
            get
            {
                return m_strUsername;
            }
            set
            {
                m_strUsername = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get
            {
                return m_strPassword;
            }
            set
            {
                m_strPassword = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async Task<LoginResult> Login()
        {
			var sLoginResult = new LoginResult();
            AuthenticationModel mUser = new AuthenticationModel
            {
                Username = m_strUsername,
                Password = m_strPassword,
            };

            Console.WriteLine(mUser.Username);

			var response = await m_hClient.PostAsJsonAsync("users/login", mUser);

			if (response.IsSuccessStatusCode)
			{
                sLoginResult.Token = await response.Content.ReadAsStringAsync();
                await m_serStorage.SaveData<string>("token", sLoginResult.Token);
			}
            else
			{
				sLoginResult.ErrorMessage = "Error";
			}

            return sLoginResult;
        }
    }
}
