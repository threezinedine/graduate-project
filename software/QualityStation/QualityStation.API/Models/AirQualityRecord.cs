namespace QualityStation.API.Models
{
    public class AirQualityRecord    {
        public string Id { get; set; } = string.Empty;
        public string StationId { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string Data { get; set; } = string.Empty;
        public Station Station { get; set; } = new Station();
    }
}
