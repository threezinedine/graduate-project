using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTTUsbCLI.App.Services
{
	public interface ICLICommandService
	{
		public void AppendMessage(string strMessage);
		public List<string> GetMessages();
		public void ClearMessages();
	}
}
