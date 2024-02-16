using Newtonsoft.Json.Schema;
using QualityStation.API.Models;
using QualityStation.API.Services.DataParserService;
using QualityStation.Shared.ModelDto.RecordAttributeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityStation.API.Test.Services
{
    public class DataParserServiceTest
    {
        [Fact]
        public async void GivenAListOfPropertiesConvertedToBytesArray_WhenDecodeAgain_ThenReturnsTheseValue()
        {
            // Arrange
            IDataParserService serDataParser = new DataParserService();
            sbyte temperature = 23;
            float humidity = 77.45f;
            uint counting = 12;
            short number = 2;

            byte[] bytes = new byte[32];

            bytes[0] = (byte)temperature;
            Array.ConstrainedCopy(BitConverter.GetBytes(humidity), 0, bytes, 1, 4);
            Array.ConstrainedCopy(BitConverter.GetBytes(number), 0, bytes, 5, 2);
            Array.ConstrainedCopy(BitConverter.GetBytes(counting), 0, bytes, 7, 4);


            var TemperatureAttribute = new RecordAttribute
            {
                AttributeIndex = 0,
                AttributeName = "Temperature",
                DataType = RecordDataType.Byte,
            };

            var HumidityAttribute = new RecordAttribute
            {
                AttributeIndex = 1,
                AttributeName = "Humidity",
                DataType = RecordDataType.Float32,
            };

            var CountingAttribute = new RecordAttribute
            {
                AttributeIndex = 3,
                AttributeName = "Counting",
                DataType = RecordDataType.UInt32,
            };

            var NumberAttribute = new RecordAttribute
            {
                AttributeIndex = 2,
                AttributeName = "Number",
                DataType = RecordDataType.Int16,
            };

            // Act
            Dictionary<string, object> jsonObject = serDataParser.Parse(
                string.Join(" ", bytes.Select(b => b.ToString("X2"))), 
                new List<RecordAttribute>
				{
					HumidityAttribute,
					TemperatureAttribute,
					CountingAttribute,
					NumberAttribute,
				});

            // Assert
            jsonObject[TemperatureAttribute.AttributeName].Should().Be(temperature);
            jsonObject[HumidityAttribute.AttributeName].Should().Be(humidity);
            jsonObject[CountingAttribute.AttributeName].Should().Be(counting);
            jsonObject[NumberAttribute.AttributeName].Should().Be(number);
        }
    }
}
