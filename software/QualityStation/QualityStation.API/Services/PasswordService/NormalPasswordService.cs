namespace QualityStation.API.Services.PasswordService
{
	public class NormalPasswordService : IPasswordService
	{
		public bool Compare(string strEncryptedPassword, string strNormalPassword)
		{
			return strEncryptedPassword.Equals(strNormalPassword);
		}

		public string EncryptPassword(string strPassword)
		{
			return strPassword;
		}
	}
}
