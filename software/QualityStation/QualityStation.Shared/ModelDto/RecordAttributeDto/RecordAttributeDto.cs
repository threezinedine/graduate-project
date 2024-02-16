using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.Shared.ModelDto.RecordAttributeDto
{
    public enum RecordDataType
    {
        Byte,
        Int16,
        Int32,
        UInt16,
        UInt32,
        Float32,
    }

    public class RecordAttributeDto
    {
        public string Id { get; set; } = string.Empty;
        public string StationId { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public int AttributeIndex { get; set; } = 0;
        public RecordDataType DataType { get; set; }
    }
}
