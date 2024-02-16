using QualityStation.Shared.ModelDto.RecordAttributeDto;
using System.ComponentModel.DataAnnotations;

namespace QualityStation.API.Models
{
    public class RecordAttribute
    {
        public string Id { get; set; } = string.Empty;
        public string StationId { get; set; } = string.Empty;
        public int AttributeIndex { get; set; } = 0;
        public string AttributeName { get ; set; } = string.Empty;
        [Required]
        public RecordDataType DataType { get; set; }
        public Station Station { get; set; } = new Station();
    }
}
