using QualityStation.API.Models;

namespace QualityStation.API.Services.UserDbService
{
	public interface IUserDbService
	{
		public Task<List<User>> GetUsersAsync();
		public Task<User?> GetUserByUsername(string strUsername);
		public Task<User?> GetUserById(string strUserId);
		public Task<string?> AddNewUser(User mUser);
		public Task<string?> UpdateUser(User mUser);
		public Task<string?> AttachNewStation(string strUserId, Station mStation);
		public Task<string?> DetachStation(string strUserId, string strStationId);
	}
}
