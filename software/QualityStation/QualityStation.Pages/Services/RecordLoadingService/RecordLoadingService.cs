using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.Pages.Services.RecordLoadingService
{
    using QualityStation.Shared.Pages.Services.AuthorizedHttpClientService;
    using System.Text.Json;

    public class RecordLoadingService
    {
		private readonly AuthorizedHttpClientService m_authorizedHttpClientService;
        private List<Dictionary<string, object>> m_Records = new List<Dictionary<string, object>>();
        public List<string> RecordKeys
        {
            get
            {
                if (m_Records.Count == 0)
                {
                    return new List<string>();
                }
                else
                {
                    return m_Records[0].Keys.ToList();
                }
            }
        }

        public List<Dictionary<string, object>> Records
        {
            get => m_Records;
        }

        public RecordLoadingService(AuthorizedHttpClientService authorizedHttpClientService)
        {
            m_authorizedHttpClientService = authorizedHttpClientService; 
        }

        public async Task<string?> LoadRecordsById(string strStationId)
        {
            await m_authorizedHttpClientService.AddToken();

            var response = await m_authorizedHttpClientService.HttpClient
                                    .GetAsync($"stations/{strStationId}/records/20/0");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                m_Records = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(content!)!;

                return null;
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
