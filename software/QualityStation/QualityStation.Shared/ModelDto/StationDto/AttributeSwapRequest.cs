using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.ModelDto.StationDto
{
    public class AttributeSwapRequest
    {
        public string StationId { get; set; } = string.Empty;
        public string FirstAttributeName { get; set; } = string.Empty;
        public string SecondAttributeName { get; set; } = string.Empty;
    }
}
