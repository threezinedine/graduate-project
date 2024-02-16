using QualityStation.API.Controllers;
using QualityStation.API.Models;
using QualityStation.API.Services.Common;
using QualityStation.API.Services.UserDbService;
using QualityStation.Shared.ModelDto.RecordAttributeDto;
using System.Reflection;

namespace QualityStation.API.Services.StationDbService
{
    public class TestStationDbService : IStationDbService
    {
        private readonly IUserDbService m_serUserDbService;
        private List<Station> m_mStations = new List<Station>();
        public TestStationDbService(IUserDbService userDbService)
        {
            m_serUserDbService = userDbService;
        }

        public Task<string?> AddAttribute(RecordAttribute mAttribute)
        {
            var station = m_mStations.FirstOrDefault(station => station.Id == mAttribute.StationId);

            var mExistedAttribute = station?.Attributes
                                    .FirstOrDefault(att => att.AttributeName == mAttribute.AttributeName);

            if (mExistedAttribute != null)
            {
                return Task.FromResult<string?>(StationControllerConstant.ATTRIBUTE_EXISTED_ERROR_MESSAGE);
            }

            station?.Attributes.Add(mAttribute);

            return Task.FromResult<string?>(null);
        }

        public async Task<string?> AddNewStation(Station mStation)
        {
            var mUser = await m_serUserDbService.GetUserByUsername(mStation.Users[0].Username)!;

            await m_serUserDbService.AttachNewStation(mUser?.Id!, mStation);

            m_mStations.Add(mStation);
            return null;
        }

        public Task<Station?> GetStationByStationId(string strStationId)
        {
            var mStation = m_mStations.FirstOrDefault(station => station.Id == strStationId);

            if (mStation == null) return Task.FromResult<Station?>(null);

            return Task.FromResult<Station?>(new Station
            {
                Id = mStation.Id,
                StationName = mStation.StationName,
                StationPosition = mStation.StationPosition,
                Attributes = mStation.Attributes.ConvertAll(att => new RecordAttribute
                {
                    Id = att.Id,
                    AttributeIndex = att.AttributeIndex,
                    AttributeName = att.AttributeName,
                    DataType = att.DataType,
                    Station = att.Station,
                    StationId = att.StationId,
                }),
                Records = mStation.Records.ConvertAll(record => new AirQualityRecord
                {
                    Id = record.Id,
                    CreatedTime = record.CreatedTime,
                    Data = record.Data,
                    StationId = record.StationId,
                    Station = record.Station,
                }),
            });
        }

        public Task<Station?> GetStationByStationName(string strStationName)
        {
            var mStation = m_mStations.FirstOrDefault(station => station.StationName == strStationName);

            if (mStation == null) return Task.FromResult<Station?>(null);

            return Task.FromResult<Station?>(new Station
            {
                Id = mStation.Id,
                StationName = mStation.StationName,
                StationPosition = mStation.StationPosition,
                Attributes = mStation.Attributes.ConvertAll(att => new RecordAttribute
                {
                    Id = att.Id,
                    AttributeIndex = att.AttributeIndex,
                    AttributeName = att.AttributeName,
                    DataType = att.DataType,
                    Station = att.Station,
                    StationId = att.StationId,
                }),
                Records = mStation.Records.ConvertAll(record => new AirQualityRecord
                {
                    Id = record.Id,
                    CreatedTime = record.CreatedTime,
                    Data = record.Data,
                    StationId = record.StationId,
                    Station = record.Station,
                }),
            });
        }

        public Task<string?> RemoveAttribute(string strStationString, string strAttributeId)
        {
            var station = m_mStations.FirstOrDefault(station => station.Id == strStationString);

            var mExistedAttribute = station?.Attributes
                        .FirstOrDefault(attribute => attribute.Id == strAttributeId);

            if (mExistedAttribute == null)
            {
                return Task.FromResult<string?>(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            station?.Attributes.RemoveAll(att => att.Id == strAttributeId);

            return Task.FromResult<string?>(null);
        }

        public Task<string?> UpdateStation(Station mStation)
        {
            int index = m_mStations.FindIndex(station => station.Id == mStation.Id);

            if (index == -1)
            {
                return Task.FromResult<string?>(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            m_mStations[index] = mStation;

            return Task.FromResult<string?>(null);
        }

        public Task<string?> UpdateAttribute(RecordAttribute mAttribute)
        {
            var station = m_mStations.FirstOrDefault(station => station.Id == mAttribute.StationId);

            if (station == null) return Task.FromResult<string?>(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);

            var attribute = station.Attributes.FirstOrDefault(att => att.Id == mAttribute.Id);

            if (attribute == null) return Task.FromResult<string?>(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);

            attribute.AttributeName = mAttribute.AttributeName;
            attribute.DataType = mAttribute.DataType;

            return Task.FromResult<string?>(null);
        }

        public Task<string?> UpdateAttributeIndex(RecordAttribute mAttribute)
        {
            var station = m_mStations.FirstOrDefault(station => station.Id == mAttribute.StationId);

            if (station == null) return Task.FromResult<string?>(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);

            var attribute = station.Attributes.FirstOrDefault(att => att.Id == mAttribute.Id);

            if (attribute == null) return Task.FromResult<string?>(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);

            attribute.AttributeIndex = mAttribute.AttributeIndex;

            return Task.FromResult<string?>(null);
        }

        public Task<string?> AddRecord(AirQualityRecord record)
        {
            var station = m_mStations.FirstOrDefault(station => station.Id == record.StationId);

            if (station == null) return Task.FromResult<string?>(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);

            station.Records.Add(record);

            return Task.FromResult<string?>(null);
        }

        public async Task<List<AirQualityRecord>> GetRecordsByStationId(string strStationId, int size, int index)
        {
            var station = await GetStationByStationId(strStationId);

            if (station == null) return new List<AirQualityRecord>();

            return station.Records.ConvertAll(record => new AirQualityRecord
            {
                Id = record.Id,
                StationId = record.StationId,
                CreatedTime = record.CreatedTime,
                Data = record.Data,
                Station = record.Station,
            });
        }

		public Task<RecordAttribute?> GetAttributeByStationIdAndAttributeName(string strStationId, string strAttributeName)
		{
            var station = m_mStations.FirstOrDefault(station => station.Id == strStationId);

            return Task.FromResult(station.Attributes.FirstOrDefault(att => att.AttributeName == strAttributeName));
		}
	}
}
