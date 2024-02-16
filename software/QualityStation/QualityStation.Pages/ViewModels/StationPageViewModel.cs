using QualityStation.Shared.ModelDto.StationDto;
using QualityStation.Shared.Pages.Pages;
using QualityStation.Shared.Pages.Services.AuthorizedHttpClientService;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.Pages.ViewModels
{
    public class StationPageViewModel
    {
        private readonly AuthorizedHttpClientService m_authorizedHttpClientService;

        private List<StationDto> m_mStations = new List<StationDto>();

        public List<StationDto> Stations
        {
            get => m_mStations;
        }

        public StationPageViewModel(AuthorizedHttpClientService authorizedHttpClientService)
        {
            m_authorizedHttpClientService = authorizedHttpClientService; 
        }

        public async Task LoadAllStations()
        {
            await m_authorizedHttpClientService.AddToken();

            var response = await m_authorizedHttpClientService.HttpClient.GetAsync("/stations");

            if (response.IsSuccessStatusCode)
            {
				m_mStations = await response.Content.ReadFromJsonAsync<List<StationDto>>();
            }
        }
    }
}
