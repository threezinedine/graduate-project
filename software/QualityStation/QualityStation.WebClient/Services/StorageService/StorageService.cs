using Blazored.LocalStorage;
using QualityStation.Shared.Pages.Services.StorageService;

namespace QualityStation.WebClient.Services.StorageService
{
	public class StorageService : IStorageService
	{
		private readonly ILocalStorageService m_serLocalStorage;
        public StorageService(ILocalStorageService localStorageService)
        {
			m_serLocalStorage = localStorageService; 
        }

		public async Task<T?> GetData<T>(string strKey)
		{
			return await m_serLocalStorage.GetItemAsync<T>(strKey);
		}

		public async Task RemoveData<T>(string strKey)
		{
			await m_serLocalStorage.RemoveItemAsync(strKey);
		}

		public async Task SaveData<T>(string strKey, T obj)
		{
			await m_serLocalStorage.SetItemAsync<T>(strKey, obj);
		}
	}
}
