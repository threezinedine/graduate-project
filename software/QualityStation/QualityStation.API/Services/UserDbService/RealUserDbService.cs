using Microsoft.EntityFrameworkCore;
using QualityStation.API.Controllers;
using QualityStation.API.Models;

namespace QualityStation.API.Services.UserDbService
{
    public class RealUserDbService : IUserDbService
	{
		private readonly StationContext m_cStationContext;
        public RealUserDbService(StationContext stationContext)
        {
			m_cStationContext = stationContext; 
        }

        public async Task<string?> AddNewUser(User mUser)
		{
			try
			{
				m_cStationContext.Users.Add(mUser);
				await m_cStationContext.SaveChangesAsync();
				return null;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

        public async Task<string?> AttachNewStation(string strUserId, Station mStation)
        {
			try
			{
				var mUser = await m_cStationContext.Users
                                .Include(user => user.Stations)
                                .FirstOrDefaultAsync(user => user.Id == strUserId);

				if (mUser == null) return UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE;

				mUser.Stations.Add(mStation);

				await m_cStationContext.SaveChangesAsync();

				return null;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
        }

        public async Task<string?> DetachStation(string strUserId, string strStationId)
        {
			try
			{
				var mUser = await m_cStationContext.Users
									.Include(user => user.Stations)
									.FirstOrDefaultAsync(user => user.Id == strUserId);

				if (mUser == null) return UserControllerConstant.USER_DOES_NOT_EXIST_ERROR_MESSAGE;

				mUser.Stations.RemoveAll(station => station.Id == strStationId);

				await m_cStationContext.SaveChangesAsync();

				return null;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
        }

        public Task<User?> GetUserById(string strUserId)
        {
			return m_cStationContext.Users
					.Include(user => user.Stations)
						.ThenInclude(station => station.Attributes)
					.FirstOrDefaultAsync(user => user.Id == strUserId);
        }

        public Task<User?> GetUserByUsername(string strUsername)
        {
			return m_cStationContext.Users
					.Include(user => user.Stations)
						.ThenInclude(station => station.Attributes)
					.FirstOrDefaultAsync(user => user.Username == strUsername);
        }

        public Task<List<User>> GetUsersAsync()
		{
			return m_cStationContext.Users
                    .Include(user => user.Stations)
                        .ThenInclude(station => station.Attributes)
                    .ToListAsync();
		}

		public async Task<string?> UpdateUser(User mUser)
		{
			var mExistedUser = await m_cStationContext.Users
								.FirstOrDefaultAsync(user => user.Id == mUser.Id);

			if (mExistedUser == null)
			{
				return "Not found.";
			}
			else
			{
				try
				{
					mExistedUser.Email = mUser.Email;
					await m_cStationContext.SaveChangesAsync();
					return null;
				}
				catch (Exception e)
				{
					return e.Message;
				}
			}
		}
	}
}
