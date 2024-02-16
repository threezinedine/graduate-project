using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTTUsbCLI.App.Services
{
	public interface ICommandHistoryService
	{
		public string GetCurrentCommand();
		public void ToPreviousCommand();
		public void ToNextCommand();
		public void AppendCommand(string strCommand);
		public bool Empty();
	}
}
