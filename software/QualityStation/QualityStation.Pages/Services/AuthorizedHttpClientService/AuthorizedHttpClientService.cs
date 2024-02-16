using QualityStation.Shared.Pages.Services.StorageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.Pages.Services.AuthorizedHttpClientService
{
	public class AuthorizedHttpClientService
	{
		private readonly HttpClient m_httpClient;
        private readonly IStorageService m_serStorage;

        public AuthorizedHttpClientService(HttpClient httpClient, IStorageService storageService)
        {
            m_httpClient = httpClient; 
            m_serStorage = storageService;
        }

        public HttpClient HttpClient
        {
            get => m_httpClient;
        }

        public async Task AddToken()
        {
            var strToken = await m_serStorage.GetData<string>("token");

            if (!string.IsNullOrEmpty(strToken)) 
            {
                m_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", strToken);
			}
        }
    }
}
