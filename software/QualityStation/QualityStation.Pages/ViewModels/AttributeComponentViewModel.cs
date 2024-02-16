using QualityStation.Shared.ModelDto.RecordAttributeDto;
using QualityStation.Shared.ModelDto.StationDto;
using QualityStation.Shared.Pages.Services.AuthorizedHttpClientService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.Pages.ViewModels
{
	public class AttributeComponentViewModel : INotifyPropertyChanged
	{
		private readonly AuthorizedHttpClientService m_authorizedHttpClientService;

        private RecordAttributeDto m_attribute = new RecordAttributeDto();

		public event PropertyChangedEventHandler? PropertyChanged;

		public string AttributeName
		{
			get => m_attribute.AttributeName;
			set
			{
				m_attribute.AttributeName = value;
				OnPropertyChanged();
			}
		}

		public RecordDataType DataType
		{
			get => m_attribute.DataType;
			set
			{
				m_attribute.DataType = value;
				OnPropertyChanged();
			}
		}
		public AttributeComponentViewModel(AuthorizedHttpClientService authorizedHttpClientService)
        {
            m_authorizedHttpClientService = authorizedHttpClientService; 
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async Task<string?> LoadAttribute(string stationId, string attriubteName)
        {
            await m_authorizedHttpClientService.AddToken();

            var response = await m_authorizedHttpClientService.HttpClient.GetAsync(
                    $"stations/attribute?stationId={stationId}&attributeName={attriubteName}");

            if (response.IsSuccessStatusCode)
            {
                m_attribute = await response.Content.ReadFromJsonAsync<RecordAttributeDto>();
                return null;
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<string?> UpdateAttribute()
        {
            await m_authorizedHttpClientService.AddToken();

            var response = await m_authorizedHttpClientService.HttpClient
                                .PutAsJsonAsync("stations/attribute/update", m_attribute);

            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string?> SwapAttribute(string stationId, string attributeName)
        {
            await m_authorizedHttpClientService.AddToken();

            var response = await m_authorizedHttpClientService.HttpClient
                                    .PutAsJsonAsync("stations/attribute/swap", new AttributeSwapRequest
                                    {
                                        StationId = stationId,
                                        FirstAttributeName = attributeName,
                                        SecondAttributeName = m_attribute.AttributeName,
                                    });

            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            else
            {
                return await response.Content.ReadAsStringAsync();  
            }
        }
    }
}
