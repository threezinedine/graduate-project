using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.ModelDto.StationDto
{
    using QualityStation.Shared.ModelDto.RecordAttributeDto;
    public class StationDto
    {
        public string Id { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string? StationPosition { get; set; } = string.Empty;
        public List<RecordAttributeDto> Attributes { get; set; } = new List<RecordAttributeDto>();
        public List<Dictionary<string, object>> RecordDicts { get; set; } = new List<Dictionary<string, object>>();
    }
}
