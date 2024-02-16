using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.ModelDto.AirQualityRecordDto
{
    public class AirQualityRecordDto
    {
        public string StationId { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string Data { get; set; } = string.Empty;
    }
}
