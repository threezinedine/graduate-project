using QualityStation.API.Models;
using QualityStation.Shared.ModelDto.RecordAttributeDto;

namespace QualityStation.API.Services.DataParserService
{
    public class DataParserService : IDataParserService
    {
        public Dictionary<string, object> Parse(string data, List<RecordAttribute> mAttributes)
        {
            data = data.TrimStart();
			string[] hexValues = data.Split(' ');
			byte[] byteArray = hexValues.Select(hex => Convert.ToByte(hex, 16)).ToArray();

			mAttributes = mAttributes.OrderBy(attribute => attribute.AttributeIndex).ToList();
            Dictionary<string, object> dResult = new Dictionary<string, object>();

            short index = 0;

            foreach (RecordAttribute attribute in mAttributes)
            {
                switch (attribute.DataType)
                {
                    case RecordDataType.Byte:
                        dResult[attribute.AttributeName] = byteArray[index];
                        index++;
                        break;
                    case RecordDataType.Float32:
                        dResult[attribute.AttributeName] = BitConverter.ToSingle(byteArray, index);
                        index += 4;
                        break;
                    case RecordDataType.Int32:
                        dResult[attribute.AttributeName] = BitConverter.ToInt32(byteArray, index);
                        index += 4;
                        break;
                    case RecordDataType.Int16:
                        dResult[attribute.AttributeName] = BitConverter.ToInt16(byteArray, index);
                        index += 2;
                        break;
                    case RecordDataType.UInt32:
                        dResult[attribute.AttributeName] = BitConverter.ToUInt32(byteArray, index);
                        index += 4;
                        break;
                    case RecordDataType.UInt16:
                        dResult[attribute.AttributeName] = BitConverter.ToUInt16(byteArray, index);
                        index += 2;
                        break;
                    default:
                        break;
                }
            }

            return dResult;
        }
    }
}
