using Microsoft.EntityFrameworkCore;
using QualityStation.API.Controllers;
using QualityStation.API.Models;
using QualityStation.API.Services.Common;
using QualityStation.Shared.ModelDto.RecordAttributeDto;
using System.Linq.Expressions;

namespace QualityStation.API.Services.StationDbService
{
    public class RealStationDbService : IStationDbService
    {
        private readonly StationContext m_stationContext;
        public RealStationDbService(StationContext stationContext)
        {
            m_stationContext = stationContext;
        }
        public Task<Station?> GetStationByStationId(string strStationId)
        {
            return m_stationContext.Stations
                            .Include(station => station.Attributes)
                            .FirstOrDefaultAsync(station => station.Id == strStationId);
        }

        public Task<Station?> GetStationByStationName(string strStationName)
        {
            return m_stationContext.Stations
                        .Include (station => station.Attributes)
                        .FirstOrDefaultAsync(station => station.StationName == strStationName);
        }

        public async Task<string?> AddNewStation(Station mStation)
        {
            try
            {
                m_stationContext.Stations.Add(mStation);

                await m_stationContext.SaveChangesAsync();
                return null;
            }
            catch (Exception ex) 
            {
                return ex.Message;
            }
        }

        public async Task<string?> UpdateStation(Station mStation)
        {
            try
            {
                var station = await m_stationContext.Stations
                                .FirstOrDefaultAsync(station => station.Id == mStation.Id);

                if (station == null) return StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE;

                station.StationPosition = mStation.StationPosition;

                await m_stationContext.SaveChangesAsync();

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string?> AddAttribute(RecordAttribute mAttribute)
        {
            var mStation = await m_stationContext.Stations
                                .Include (station => station.Attributes)
                                .FirstOrDefaultAsync(station => station.Id == mAttribute.StationId);

            if (mStation == null) return StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE;

            try
            {
                mStation.Attributes.Add(mAttribute);

                await m_stationContext.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string?> RemoveAttribute(string strStationId, string strAttributeId)
        {
            var mStation = await m_stationContext.Stations
                                .Include(station => station.Attributes)
                                .FirstOrDefaultAsync(station => station.Id == strStationId);

            if (mStation == null) return StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE;

            var mAttribute = mStation.Attributes.FirstOrDefault(att => att.Id == strAttributeId);

            if (mAttribute == null) return StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE;

            try
            {
                mStation.Attributes.Remove(mAttribute);

                await m_stationContext.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string?> UpdateAttribute(RecordAttribute mAttribute)
        {
            var mExistedAttr = await m_stationContext.RecordAttributes
                                .FirstOrDefaultAsync(att => att.Id == mAttribute.Id);

            if (mExistedAttr == null) return StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE;

            try
            {
                mExistedAttr.AttributeName = mAttribute.AttributeName;
                mExistedAttr.DataType = mAttribute.DataType;

                await m_stationContext.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string?> UpdateAttributeIndex(RecordAttribute mAttribute)
        {
            var mExistedAttr = await m_stationContext.RecordAttributes
                                .FirstOrDefaultAsync(att => att.Id == mAttribute.Id);

            if (mExistedAttr == null) return StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE;

            try
            {
                mExistedAttr.AttributeIndex = mAttribute.AttributeIndex;

                await m_stationContext.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string?> AddRecord(AirQualityRecord record)
        {
            var station = await m_stationContext.Stations
                                .Include(station => station.Records)
                            .FirstOrDefaultAsync(station => station.Id == record.StationId);

            if (station == null) return StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE;

            try
            {
                station.Records.Add(record);

                await m_stationContext.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Task<List<AirQualityRecord>> GetRecordsByStationId(string strStationId, int size, int index)
        {
            return m_stationContext.AirQualityRecords
                    .Where(record => record.StationId == strStationId)
                    .OrderByDescending(record => record.CreatedTime) 
					.Take(size)
                    .ToListAsync();
        }

		public Task<RecordAttribute?> GetAttributeByStationIdAndAttributeName(string strStationId, string strAttributeName)
		{
            return m_stationContext.RecordAttributes.FirstOrDefaultAsync(
                att => att.StationId == strStationId && att.AttributeName == strAttributeName);

		}
	}
}
