using QualityStation.API.Models;
using QualityStation.API.Services.Common;
using QualityStation.Shared.ModelDto.RecordAttributeDto;
using QualityStation.Shared.ModelDto.StationDto;

namespace QualityStation.API.Services.StationDbService
{
    public interface IStationDbService
    {
        public Task<string?> AddNewStation(Station mStation);
        public Task<Station?> GetStationByStationName(string strStationName);
        public Task<Station?> GetStationByStationId(string strStationId);
        public Task<RecordAttribute?> GetAttributeByStationIdAndAttributeName(string strStationId, string strAttributeName);

		public Task<List<AirQualityRecord>> GetRecordsByStationId(string strStationId, int size, int index);
        public Task<string?> UpdateStation(Station mStation);
        public Task<string?> AddAttribute(RecordAttribute mAttribute);
        public Task<string?> RemoveAttribute(string strStationId, string strAttributeId);
        public Task<string?> UpdateAttributeIndex(RecordAttribute mAttribute);
        public Task<string?> UpdateAttribute(RecordAttribute mAttribute);
        public Task<string?> AddRecord(AirQualityRecord record);
    }
}
