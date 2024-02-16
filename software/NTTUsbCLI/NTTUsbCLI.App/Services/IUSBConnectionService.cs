using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTTUsbCLI.App.Services
{
	public interface IUSBConnectionService
	{
		public Task<List<string>> GetCurrentPorts();
		public Task ConnectPort(string strPort);
		public bool CheckFree();
		public string? GetConnectedCOM();
		public Task<string?> QueryCommand(List<string> strCommand);
	}
}
