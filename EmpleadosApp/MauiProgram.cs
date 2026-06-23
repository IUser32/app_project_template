using Microsoft.Extensions.Logging;

namespace EmpleadosApp;

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
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		ConfigurarInputsSinUnderline();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	private static void ConfigurarInputsSinUnderline()
	{
#if ANDROID
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("SinUnderline", (handler, view) =>
		{
			handler.PlatformView.BackgroundTintList =
				Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
		});

		Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("SinUnderline", (handler, view) =>
		{
			handler.PlatformView.BackgroundTintList =
				Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
		});

		Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("SinUnderline", (handler, view) =>
		{
			handler.PlatformView.BackgroundTintList =
				Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
		});

		Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("SinUnderline", (handler, view) =>
		{
			handler.PlatformView.BackgroundTintList =
				Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
		});
#endif
	}
}
