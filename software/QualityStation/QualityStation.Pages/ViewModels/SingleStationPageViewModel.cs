using QualityStation.Shared.ModelDto.RecordAttributeDto;
using QualityStation.Shared.ModelDto.StationDto;
using QualityStation.Shared.Pages.Services.AuthorizedHttpClientService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QualityStation.Shared.Pages.ViewModels
{
	public class SingleStationPageViewModel
	{
		private readonly AuthorizedHttpClientService m_authorizedHttpClientService;

        private StationDto m_mStation = new StationDto();
        private List<Dictionary<string, object>> m_Records = new List<Dictionary<string, object>>();

        public StationDto Station
        {
            get => m_mStation;
        }

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

        public ObservableCollection<RecordAttributeDto> Attributes
        {
            get => new ObservableCollection<RecordAttributeDto>(m_mStation.Attributes);
        }

        public SingleStationPageViewModel(AuthorizedHttpClientService authorizedHttpClientService)
        {
            m_authorizedHttpClientService = authorizedHttpClientService; 
        }

        public async Task<string?> LoadRecordsById(string strStationId)
        {
			await m_authorizedHttpClientService.AddToken();

			var response = await m_authorizedHttpClientService.HttpClient
									.GetAsync($"stations/{strStationId}/records/10/0");

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

        public async Task<string?> LoadStationById(string strStationId)
        {
            await m_authorizedHttpClientService.AddToken();

            var response = await m_authorizedHttpClientService.HttpClient
                                    .GetAsync($"stations/{strStationId}");

            if (response.IsSuccessStatusCode)
            {
				m_mStation = await response.Content.ReadFromJsonAsync<StationDto>();
                m_mStation.Attributes = m_mStation.Attributes.OrderBy(att => att.AttributeIndex).ToList();

                return await LoadRecordsById(strStationId);
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
