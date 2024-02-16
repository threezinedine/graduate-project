using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QualityStation.API.Models;
using QualityStation.API.Services.Common;
using QualityStation.API.Services.DataParserService;
using QualityStation.API.Services.StationDbService;
using QualityStation.API.Services.UserDbService;
using QualityStation.Shared.ModelDto.AirQualityRecordDto;
using QualityStation.Shared.ModelDto.RecordAttributeDto;
using QualityStation.Shared.ModelDto.StationDto;
using System.Runtime.InteropServices;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace QualityStation.API.Controllers
{
    public class StationControllerConstant
    {
        public const string SERVER_ERROR_ERROR_MESSAGE = "Server Error.";
        public const string STATION_EXISTED_ERROR_MESSAGE = "Station Existed.";
        public const string STATION_DOES_NOT_EXIST_ERROR_MESSAGE = "Station does not exist.";
        public const string STATION_IS_NOT_ATTACHED_ERROR_MESSAGE = "Station is not attached.";
        public const string ATTRIBUTE_EXISTED_ERROR_MESSAGE = "Attribute existed.";
        public const string ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE = "Attribute does not exist.";
    }

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("stations")]
    public class StationController : ControllerBase
    {
        private readonly IUserDbService m_serUserDbService;
        private readonly IMapper m_iMapper;
        private readonly IStationDbService m_serStationDbService;
        private readonly IDataParserService m_serDataParserService;

        public StationController(IMapper mapper, 
                                    IStationDbService stationDbService,
                                    IUserDbService userDbService,
                                    IDataParserService dataParserService)
        {
            m_iMapper = mapper;
            m_serStationDbService = stationDbService;
            m_serUserDbService = userDbService;
            m_serDataParserService = dataParserService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StationDto>>> QueryAllStations()
        {
            var mUser = await m_serUserDbService.GetUserByUsername(HttpContext.User?.Identity?.Name!);

            if (mUser == null)
            {
                return BadRequest(StationControllerConstant.SERVER_ERROR_ERROR_MESSAGE);
            }

            return Ok(mUser.Stations.ConvertAll(station => m_iMapper.Map<StationDto>(station)));
        }

        [HttpGet("{stationId}")]
        public async Task<ActionResult<StationDto>> GetStationById(string stationId)
        {
            var mStation = await m_serStationDbService.GetStationByStationId(stationId);

            if (mStation == null)
            {
                return NotFound(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            var responseStation = m_iMapper.Map<StationDto>(mStation);

            foreach (var records in mStation.Records)
            {
				var parsedRecord = m_serDataParserService.Parse(records.Data, mStation.Attributes);
                parsedRecord["Created"] = records.CreatedTime;
				responseStation.RecordDicts.Add(parsedRecord);
			}

            return Ok(responseStation);
        }

        [HttpPost]
        public async Task<ActionResult<StationDto>> CreateStation(StationDto mStationDto)
        {
            var mUser = await m_serUserDbService.GetUserByUsername(HttpContext.User?.Identity?.Name!);

            if (mUser == null)
            {
                return BadRequest(StationControllerConstant.SERVER_ERROR_ERROR_MESSAGE);
            }

            var mExistedStation= await m_serStationDbService.GetStationByStationName(mStationDto.StationName);

            if (mExistedStation != null)
            {
                return Conflict(StationControllerConstant.STATION_EXISTED_ERROR_MESSAGE);
            }

            var newStation = new Station
            {
                Id = Guid.NewGuid().ToString(),
                StationName = mStationDto.StationName,
                StationPosition = mStationDto.StationPosition!,
                Users = new List<User>
                {
                    mUser,
                },
            };

            var strErrorMessage = await m_serStationDbService.AddNewStation(newStation);

            if (strErrorMessage != null)
            {
                return BadRequest(StationControllerConstant.SERVER_ERROR_ERROR_MESSAGE);
            }

            var mCreatedStation = await m_serStationDbService.GetStationByStationName(mStationDto.StationName);

            return Ok(m_iMapper.Map<StationDto>(mCreatedStation));
        }

        [HttpPut("attribute/add")]
        public async Task<ActionResult<StationDto>> AddAttribute(RecordAttributeDto attributeDto)
        {
            var station = await m_serStationDbService.GetStationByStationId(attributeDto.StationId);

            if (station == null)
            {
                return NotFound(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            var mExistedAttribue = station.Attributes.FirstOrDefault(att => att.AttributeName == attributeDto.AttributeName);

            if (mExistedAttribue != null)
            {
                return Conflict(StationControllerConstant.ATTRIBUTE_EXISTED_ERROR_MESSAGE);
            }

            var mAddedAttribute = new RecordAttribute
            {
                Id = Guid.NewGuid().ToString(),
                StationId = station.Id,
                AttributeName = attributeDto.AttributeName,
                DataType = attributeDto.DataType,
                AttributeIndex = station.Attributes.Count(),
            };

            var strErrorMessage = await m_serStationDbService.AddAttribute(mAddedAttribute);

            if (strErrorMessage != null)
            {
                return BadRequest(strErrorMessage);
            }

            var mStationResult = await m_serStationDbService.GetStationByStationId(attributeDto.StationId);

            return Ok(m_iMapper.Map<StationDto>(mStationResult));
        }

        [HttpPut("attribute/remove")]
        public async Task<ActionResult<StationDto>> RemoveAttribute( RecordAttributeDto attributeDto)
        {
            var station = await m_serStationDbService.GetStationByStationId(attributeDto.StationId);

            if (station == null)
            {
                return NotFound(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            station.Attributes = station.Attributes.OrderBy(att => att.AttributeIndex).ToList();
            var mExistedAttributeIndex = station.Attributes
                        .FindIndex(att => att.AttributeName == attributeDto.AttributeName);

            if (mExistedAttributeIndex == -1)
            {
                return NotFound(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            var strErrorMessage = await m_serStationDbService
                                            .RemoveAttribute(
                                                attributeDto.StationId,
                                                station.Attributes[mExistedAttributeIndex].Id);

            if (strErrorMessage != null)
            {
                return NotFound(strErrorMessage);
            }

            var mNewStation = await m_serStationDbService.GetStationByStationId(attributeDto.StationId);

            if (mNewStation == null)
            {
                return NotFound(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            for (int i = 0; i< mNewStation.Attributes.Count; i++)
            {
                var att = mNewStation.Attributes[i];
                att.AttributeIndex = i;
                strErrorMessage = await m_serStationDbService.UpdateAttributeIndex(att);

                if (strErrorMessage != null)
                {
                    return BadRequest(strErrorMessage);
                }
            }

            var mStationResult = await m_serStationDbService.GetStationByStationId(attributeDto.StationId);

            return Ok(m_iMapper.Map<StationDto>(mStationResult));
        }

        [HttpPut("attribute/swap")]
        public async Task<ActionResult<StationDto>> SwapAttributes(AttributeSwapRequest request)
        {
            var mStation = await m_serStationDbService.GetStationByStationId(request.StationId);
            int nTemp = -1;

            if (mStation == null)
            {
                return NotFound(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            var mFirstAttribute = mStation.Attributes
                        .FirstOrDefault(att => att.AttributeName == request.FirstAttributeName);

            if (mFirstAttribute == null)
            {
                return NotFound(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);
            }
            var mSecondAttribute = mStation.Attributes
                        .FirstOrDefault(att => att.AttributeName == request.SecondAttributeName);

            if (mSecondAttribute == null)
            {
                return NotFound(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            nTemp = mFirstAttribute.AttributeIndex;
            mFirstAttribute.AttributeIndex = mSecondAttribute.AttributeIndex;
            mSecondAttribute.AttributeIndex = nTemp;

            var strErrorMesssage = await m_serStationDbService.UpdateAttributeIndex(mFirstAttribute);
            if (strErrorMesssage != null)
            {
                return BadRequest(strErrorMesssage);
            }

            strErrorMesssage = await m_serStationDbService.UpdateAttributeIndex(mSecondAttribute);

            if (strErrorMesssage != null)
            {
                return BadRequest(strErrorMesssage);
            }

            var mStationResult = await m_serStationDbService.GetStationByStationId(request.StationId);

            return Ok(m_iMapper.Map<StationDto>(mStationResult));
        }

        [HttpPut("attribute/update")]
        public async Task<ActionResult<StationDto>> UpdateAttribute(RecordAttributeDto recordAttributeDto)
        {
            var mStation = await m_serStationDbService.GetStationByStationId(recordAttributeDto.StationId);

            if (mStation == null) return NotFound(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);

            var mExistedAttribute = mStation.Attributes
                            .FirstOrDefault(att => att.AttributeName == recordAttributeDto.AttributeName 
                                                && att.Id != recordAttributeDto.Id);


            var mAttribute = mStation.Attributes.FirstOrDefault(att => att.Id == recordAttributeDto.Id);

            if (mAttribute == null) return NotFound(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);

            if (mExistedAttribute != null) return Conflict(StationControllerConstant.ATTRIBUTE_EXISTED_ERROR_MESSAGE);

            mAttribute.AttributeName = recordAttributeDto.AttributeName;
            mAttribute.DataType = recordAttributeDto.DataType;

            string? strErrorMessage = await m_serStationDbService.UpdateAttribute(mAttribute);
            if (strErrorMessage != null) return NotFound(strErrorMessage);
            var mStationResult = await m_serStationDbService.GetStationByStationId(mAttribute.StationId);
            return Ok(m_iMapper.Map<StationDto>(mStationResult));
        }

        [HttpPut("attach")]
        public async Task<ActionResult<StationDto>> AttachStation(StationDto mStationDto)
        {
            var mUser = await m_serUserDbService.GetUserByUsername(HttpContext.User?.Identity?.Name!);
            if (mUser == null)
            {
                return NotFound(UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            var mStation = await m_serStationDbService.GetStationByStationId(mStationDto.Id);

            if (mStation == null)
            {
                return NotFound(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

			string? strErrorMessage = await m_serUserDbService.AttachNewStation(mUser.Id, mStation);

            if (strErrorMessage != null)
            {
                return BadRequest(StationControllerConstant.SERVER_ERROR_ERROR_MESSAGE);
            }

            var mStationResult = await m_serStationDbService.GetStationByStationId(mStation.Id);

            return Ok(m_iMapper.Map<StationDto>(mStationResult));
        }

        [HttpPut]
        public async Task<ActionResult<StationDto>> UpdateStation(StationDto stationDto)
        {
            var mStation = await m_serStationDbService.GetStationByStationId(stationDto.Id);

            if (mStation == null)
            {
                return NotFound(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            mStation.StationPosition = stationDto.StationPosition!;

            var strErrorMessage = await m_serStationDbService.UpdateStation(mStation);

            if (strErrorMessage != null)
            {
                return BadRequest(StationControllerConstant.SERVER_ERROR_ERROR_MESSAGE);
            }

            var mStationResult = await m_serStationDbService.GetStationByStationId(stationDto.Id);

            return Ok(m_iMapper.Map<StationDto>(mStationResult));
        }

        [HttpPut("detach")]
        public async Task<ActionResult<StationDto>> DetachStation(StationDto stationDto)
        {
            var mUser = await m_serUserDbService.GetUserByUsername(HttpContext.User?.Identity?.Name!);

            if (mUser == null)
            {
                return NotFound(UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

			var strErrorMessage = await m_serUserDbService.DetachStation(mUser.Id, stationDto.Id);

            if (strErrorMessage != null)
            {
                return BadRequest(strErrorMessage);
            }

            return Ok(new StationDto());
        }

        [HttpPost("records")]
        [AllowAnonymous]
        public async Task<ActionResult<AirQualityRecordDto>> AddRecord(AirQualityRecordDto airQualityRecordDto)
        {
            var record = new AirQualityRecord
            {
                Id = Guid.NewGuid().ToString(),
                CreatedTime = DateTime.Now,
                StationId = airQualityRecordDto.StationId,
                Data = airQualityRecordDto.Data,
            };

            var strErrorMessage = await m_serStationDbService.AddRecord(record);
            if (strErrorMessage != null)
            {
                return BadRequest(strErrorMessage);
            }
            return Ok(new AirQualityRecordDto());
        }

        [HttpGet("{stationId}/records/{size}/{index}")]
        public async Task<ActionResult<List<Dictionary<string, object>>>> GetRecords(string stationId, int size, int index)
        {
            var result = new List<Dictionary<string, object>>();
            var station = await m_serStationDbService.GetStationByStationId(stationId);

            if (station == null)
            {
                return NotFound(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
            }

            var records = await m_serStationDbService.GetRecordsByStationId(stationId, size, index);

            foreach (var record in records)
            {
                try
                {
					var parsedRecord = m_serDataParserService.Parse(record.Data, station.Attributes);
					parsedRecord["Created"] = record.CreatedTime;
					result.Add(parsedRecord);
                }
                catch
                {
                    
                }
            }

            return Ok(result);
        }

        [HttpGet("attribute")]
        public async Task<ActionResult<RecordAttributeDto>> GetAttributeByStationIdAndAttributeName(
                                                            string stationId, string attributeName)
        {
            var station = await m_serStationDbService.GetStationByStationId(stationId);

            if (station == null) return NotFound(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);

            var attribute = await m_serStationDbService.GetAttributeByStationIdAndAttributeName(stationId, attributeName);
            if (attribute == null) return NotFound(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);
            return Ok(m_iMapper.Map<RecordAttributeDto>(attribute));
        }
	}
}
