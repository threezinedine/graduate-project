namespace QualityStation.API.Services.PasswordService
{
	public interface IPasswordService
	{
		public string EncryptPassword(string strPassword);
		public bool Compare(string strEncryptedPassword, string strNormalPassword);
	}
}
