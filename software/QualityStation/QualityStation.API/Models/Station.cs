namespace QualityStation.API.Models
{
    public class Station
    {
        public string Id { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string StationPosition { get; set; } = string.Empty;
        public List<User> Users { get; set; } = new List<User>();
        public List<AirQualityRecord> Records { get; set; } = new List<AirQualityRecord>();
        public List<RecordAttribute> Attributes { get; set; } = new List<RecordAttribute>();
    }
}
