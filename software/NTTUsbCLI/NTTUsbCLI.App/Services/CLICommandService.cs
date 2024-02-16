using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTTUsbCLI.App.Services
{
	public class CLICommandService : ICLICommandService
	{
		private List<string> _strMessages = new List<string>();
		public void AppendMessage(string strMessage)
		{
			_strMessages.Add(strMessage);
		}

		public void ClearMessages()
		{
			_strMessages.Clear();
		}

		public List<string> GetMessages()
		{
			return new List<string>(_strMessages);
		}
	}
}
