using AutoMapper;
using QualityStation.API.Models;
using QualityStation.Shared.ModelDto.RecordAttributeDto;
using QualityStation.Shared.ModelDto.StationDto;
using QualityStation.Shared.ModelDto.UserDto;

namespace QualityStation.API.Helper
{
	public class APIMapper: Profile
	{
        public APIMapper()
        {
            CreateMap<User, UserInfoDto>();
            CreateMap<UserInfoDto, User>();
            CreateMap<Station, StationDto>();
            CreateMap<StationDto, Station>();
            CreateMap<RecordAttribute, RecordAttributeDto>();
            CreateMap<RecordAttributeDto, RecordAttribute>();
        }
    }
}
