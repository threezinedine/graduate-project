using QualityStation.API.Models;

namespace QualityStation.API.Services.DataParserService
{
    public interface IDataParserService
    {
        public Dictionary<string, object> Parse(string data, List<RecordAttribute> mAttributes);
    }
}
