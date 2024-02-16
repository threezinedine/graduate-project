using QualityStation.API.Controllers;
using QualityStation.API.Models;

namespace QualityStation.API.Services.UserDbService
{
	public class TestUserDbService : IUserDbService
	{
		private List<User> m_mUsers = new List<User>();

		public Task<string?> AddNewUser(User mUser)
		{
			m_mUsers.Add(mUser);
			return Task.FromResult<string?>(null);
		}

        public Task<string?> AttachNewStation(string strUserId, Station mStation)
        {
			var mUser = m_mUsers.FirstOrDefault(user => user.Id == strUserId)!;
			mUser.Stations.Add(mStation);

			return Task.FromResult<string?>(null);
        }

        public Task<string?> DetachStation(string strUserId, string strStationId)
        {
			var mUser = m_mUsers.FirstOrDefault(user => user.Id == strUserId);

			if (mUser == null)
			{
				return Task.FromResult<string?>(UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE);
			}

			bool bStationExisted = mUser.Stations.Any(station => station.Id == strStationId);

			if (bStationExisted)
			{
                mUser.Stations.RemoveAll(station => station.Id == strStationId);
			}
			else
			{
				return Task.FromResult<string?>(StationControllerConstant.STATION_IS_NOT_ATTACHED_ERROR_MESSAGE);
			}

			return Task.FromResult<string?>(null);
        }

        public Task<User?> GetUserById(string strUserId)
        {
			return Task.FromResult(m_mUsers.FirstOrDefault(user => user.Id == strUserId));
        }

        public Task<User?> GetUserByUsername(string strUsername)
        {
			return Task.FromResult(m_mUsers.FirstOrDefault(user => user.Username == strUsername));
        }

        public Task<List<User>> GetUsersAsync()
		{
			return Task.FromResult(m_mUsers.ConvertAll(m => new User
			{
				Id = m.Id,
				Email = m.Email,
				Username = m.Username,
				EncryptedPassword = m.EncryptedPassword,
				Stations = m.Stations.ConvertAll(station => new Station
				{
					Id = station.Id,
					StationName = station.StationName,
					StationPosition = station.StationPosition,
				})
			}));
		}

		public Task<string?> UpdateUser(User mUser)
		{
			int index = m_mUsers.FindIndex(user => user.Id == mUser.Id);
			m_mUsers[index] = mUser;

			return Task.FromResult<string?>(null);
		}
	}
}
