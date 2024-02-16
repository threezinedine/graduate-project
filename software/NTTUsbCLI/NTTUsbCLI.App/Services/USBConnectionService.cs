using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Microsoft.Maui.Layouts;
using Microsoft.VisualBasic;

namespace NTTUsbCLI.App.Services
{
	public class USBConnectionService : IUSBConnectionService
	{
		private string? _strConnectedCom = null;
		private SerialPort? _port = null;
		private ICLICommandService _serCommandService;
		private ManualResetEventSlim dataReceivedEvent = new ManualResetEventSlim(false);
		private string _strResponse = "";
        public USBConnectionService(ICLICommandService cLICommandService)
        {
			_serCommandService = cLICommandService; 
        }
        public bool CheckFree()
		{
			return _strConnectedCom == null;
		}

		public string? GetConnectedCOM()
		{
			if (CheckFree())
			{
				return null;
			}
			else
			{
				return _strConnectedCom;
			}
		}

		public Task ConnectPort(string strPort)
		{
			return Task.CompletedTask;
		}

		public Task<List<string>> GetCurrentPorts()
		{
			return Task.FromResult(SerialPort.GetPortNames().ToList());
		}

		public async Task<string?> QueryCommand(List<string> strCommand)
		{
			if (strCommand.Count == 0) return "";

			switch (strCommand[0])
			{
				case "ports":
					List<string> cmds = await GetCurrentPorts();
					if (cmds.Count == 0) return "Not found any ports.";
					return $"[\"{string.Join("\", \"", cmds)}\"]";
				case "cls":
					_serCommandService.ClearMessages();
					return null;
				case "status":
					if (CheckFree())
					{
						return "No connected.";
					}
					else
					{
						return $"Connected to {GetConnectedCOM()}";
					}
				case "connect":
					if (SerialPort.GetPortNames().ToList().Contains(strCommand[1]))
					{
						if (strCommand.Count == 1) return "Need port argument";
						_strConnectedCom = strCommand[1];
						_port = new SerialPort(strCommand[1], 9600);
						_port.ReadTimeout = 1000;
						_port.Parity = Parity.None;
						_port.Handshake = Handshake.None;
						_port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
						try
						{
							_port.Open();
						}
						catch (Exception ex)
						{
							return ex.Message;
						}
						return "Success";
					}
					else
					{
						return "Port not found";
					}
				case "disconnect":
					_strConnectedCom = null;
					_port.Close();
					return "Disconnected";

				default:
					if (CheckFree()) return "Not connected with any port.";
					string strResponse = "";

					try
					{
						string dataToSend = string.Join(" ", strCommand);
						_port.Write(dataToSend);
						dataReceivedEvent.Reset();

						dataReceivedEvent.Wait(TimeSpan.FromSeconds(1));
						return _strResponse;

						//_port.DiscardInBuffer();
						//byte[] buffer = new byte[128];
						//int byteReads = _port.Read(buffer, 0, 128);
						//strResponse = Encoding.UTF8.GetString(buffer);
					}
					catch (Exception ex)
					{
						strResponse = ex.Message;
					}
					return strResponse;
			}

		}
		private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			int intBuffer;
			intBuffer = _port.BytesToRead;
			byte[] byteBuffer = new byte[intBuffer];
			_port.Read(byteBuffer, 0, intBuffer);
			_strResponse = Encoding.UTF8.GetString(byteBuffer);
			dataReceivedEvent.Set();
		}
	}
}
