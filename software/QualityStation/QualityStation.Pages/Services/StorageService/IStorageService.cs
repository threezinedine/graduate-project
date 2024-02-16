using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.Pages.Services.StorageService
{
	public interface IStorageService
	{
		public Task SaveData<T>(string strKey, T obj);
		public Task<T?> GetData<T>(string strKey);
		public Task RemoveData<T>(string strKey);
	}
}
