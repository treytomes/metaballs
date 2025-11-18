using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using RetroTK.Gfx;
using RetroTK.IO;
using RetroTK.Services;

namespace RetroTK;

public static class ServiceCollectionExtensions
{
	private static readonly Version OPENGL_VERSION = new Version(4, 5);

	public static IServiceCollection AddRetroTK(this IServiceCollection @this)
	{
		// Register services.
		@this.AddSingleton<IEventBus, EventBus>();
		@this.AddSingleton<IResourceManager, ResourceManager>();
		@this.AddSingleton<IGameEngine, GameEngine>();

		// Register GameWindow as a factory.
		@this.AddSingleton(serviceProvider =>
		{
			// Get window settings from configuration.
			var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

			var nativeWindowSettings = new NativeWindowSettings()
			{
				ClientSize = new Vector2i(appSettings.Window.Width, appSettings.Window.Height),
				Title = appSettings.Window.Title,
				Profile = ContextProfile.Core,
				APIVersion = OPENGL_VERSION,
				WindowState = appSettings.Window.Fullscreen ? WindowState.Fullscreen : WindowState.Normal
			};

			return new GameWindow(GameWindowSettings.Default, nativeWindowSettings);
		});

		// Register IVirtualDisplay as a factory.
		@this.AddSingleton<IVirtualDisplay>(serviceProvider =>
		{
			// Get window settings from configuration.
			var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

			// Get or create window.
			var windowSize = new Vector2i(
				appSettings.Window.Width,
				appSettings.Window.Height
			);

			// Create virtual display settings.
			var virtualDisplaySettings = new VirtualDisplaySettings
			{
				Width = appSettings.VirtualDisplay.Width,
				Height = appSettings.VirtualDisplay.Height,
				VertexShaderPath = appSettings.VirtualDisplay.VertexShaderPath,
				FragmentShaderPath = appSettings.VirtualDisplay.FragmentShaderPath
			};

			// Create and return the virtual display.
			return new VirtualDisplay(windowSize, virtualDisplaySettings);
		});

		@this.AddSingleton<IRenderingContext, RenderingContext>();

		@this.AddTransient<IResourceLoader<Image>, ImageLoader>();
		@this.AddTransient<IGameStateManager, GameStateManager>();

		return @this;
	}
}