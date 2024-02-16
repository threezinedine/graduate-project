using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Maui.Layouts;
using NTTUsbCLI.App.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NTTUsbCLI.App.ViewModels
{
	public class CliPageViewModel : INotifyPropertyChanged
	{
		private readonly ICLICommandService _serCommandService;
		private readonly IUSBConnectionService _serUSBConnectionService;
		private readonly ICommandHistoryService _serCommandHistoryService;
		private string _strCommand = string.Empty;
		public string Command
		{
			get { return _strCommand; }
			set
			{
				_strCommand = value.Trim();
				RunCommand();
			}
		}

		public async void RunCommand()
		{
			_serCommandService.AppendMessage($">> {_strCommand.Trim()}");
			string? strResponse = await _serUSBConnectionService.QueryCommand(_strCommand.Split(' ').ToList());
			_serCommandHistoryService.AppendCommand(_strCommand);
			_strCommand = string.Empty;
			if (strResponse != null)
			{
				_serCommandService.AppendMessage(strResponse);
			}
			OnPropertyChanged();
		}

		public List<string> Messages
		{
			get { return _serCommandService.GetMessages(); }
		}

        public CliPageViewModel(ICLICommandService commandService,
								IUSBConnectionService uSBConnectionService,
								ICommandHistoryService commandHistoryService)
		{
			_serCommandService = commandService;
			_serUSBConnectionService = uSBConnectionService;
			_serCommandHistoryService = commandHistoryService;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public void NextCommand()
		{
			_serCommandHistoryService.ToNextCommand();
			if (!_serCommandHistoryService.Empty())
			{
				_strCommand = _serCommandHistoryService.GetCurrentCommand();
				OnPropertyChanged();
			}
		}

		public void PrevCommand()
		{
			_serCommandHistoryService.ToPreviousCommand();
			if (!_serCommandHistoryService.Empty())
			{
				_strCommand = _serCommandHistoryService.GetCurrentCommand();
				OnPropertyChanged();
			}
		}

		public void ClearCommand()
		{
			_strCommand = string.Empty;
			OnPropertyChanged();
		}
	}
}
