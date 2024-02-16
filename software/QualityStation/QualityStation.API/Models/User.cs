namespace QualityStation.API.Models
{
	public class User
	{
		public string Id { get; set; } = string.Empty;
		public string Username {  get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Role { get; set; } = "User";
		public string EncryptedPassword { get; set; } = string.Empty;
		public List<Station> Stations { get; set; } = new List<Station>();

		public void GenerateId()
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}
