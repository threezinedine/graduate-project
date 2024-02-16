using Microsoft.Extensions.Logging;
using NTTUsbCLI.App.Services;
using NTTUsbCLI.App.ViewModels;

namespace NTTUsbCLI.App
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

			builder.Services.AddMauiBlazorWebView();
			builder.Services.AddSingleton<ICLICommandService, CLICommandService>();
			builder.Services.AddSingleton<IUSBConnectionService, USBConnectionService>();
			builder.Services.AddSingleton<ICommandHistoryService, CommandHistoryService>();
			builder.Services.AddSingleton<CliPageViewModel>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}