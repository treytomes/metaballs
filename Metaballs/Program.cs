using Metaballs.States;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RetroTK;
using RetroTK.Services;
using RetroTK.States;
using System.CommandLine;

namespace Metaballs;

class Program
{
	static async Task<int> Main(string[] args)
	{
		// Define command-line options.
		var configFileOption = new Option<string>(
			name: "--config",
			description: "Path to the configuration file",
			getDefaultValue: () => "appsettings.json");

		var debugOption = new Option<bool>(
			name: "--debug",
			description: "Enable debug mode");

		var fullscreenOption = new Option<bool>(
			name: "--fullscreen",
			description: "Start in fullscreen mode");

		var widthOption = new Option<int?>(
			name: "--width",
			description: "Window width in pixels");

		var heightOption = new Option<int?>(
			name: "--height",
			description: "Window height in pixels");

		// Create root command.
		var rootCommand = new RootCommand("Critters Game");
		rootCommand.AddOption(configFileOption);
		rootCommand.AddOption(debugOption);
		rootCommand.AddOption(fullscreenOption);
		rootCommand.AddOption(widthOption);
		rootCommand.AddOption(heightOption);

		// Set handler for processing the command.
		rootCommand.SetHandler(static async (configFile, debug, fullscreen, width, height) =>
			{
				await RunGameAsync(configFile, debug, fullscreen, width, height);
			},
			configFileOption, debugOption, fullscreenOption, widthOption, heightOption);

		// Parse the command line.
		return await rootCommand.InvokeAsync(args);
	}

	static async Task RunGameAsync(string configFile, bool debug, bool fullscreen, int? width, int? height)
	{
		try
		{
			// Build host with DI container.
			using var host = CreateHostBuilder(configFile, debug, fullscreen, width, height).Build();

			// Start the game.
			await host.Services.GetRequiredService<IGameEngine>().RunAsync<MainState>();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"Error starting the game: {ex.Message}");
			Environment.Exit(1);
		}
	}

	static IHostBuilder CreateHostBuilder(string configFile, bool debug, bool fullscreen, int? width, int? height)
	{
		return Host.CreateDefaultBuilder()
			.ConfigureAppConfiguration((hostContext, config) =>
			{
				config.Sources.Clear();
				// config.SetBasePath(Directory.GetCurrentDirectory());
				config.SetBasePath(AppContext.BaseDirectory);
				config.AddJsonFile(configFile, optional: false, reloadOnChange: false);

				// Add command line overrides.
				var commandLineConfig = new Dictionary<string, string?>();
				if (debug)
				{
					commandLineConfig["Debug"] = "true";
				}
				if (fullscreen)
				{
					commandLineConfig["Window:Fullscreen"] = "true";
				}
				if (width.HasValue)
				{
					commandLineConfig["Window:Width"] = width.ToString();
				}
				if (height.HasValue)
				{
					commandLineConfig["Window:Height"] = height.ToString();
				}

				config.AddInMemoryCollection(commandLineConfig);
			})
			.ConfigureLogging((hostContext, logging) =>
			{
				logging.ClearProviders();
				logging.AddConsole();

				// Set minimum log level based on debug setting.
				var debugEnabled = hostContext.Configuration.GetValue<bool>("Debug");
				if (debugEnabled)
				{
					logging.SetMinimumLevel(LogLevel.Debug);
				}
				else
				{
					logging.SetMinimumLevel(LogLevel.Information);
				}
			})
			.ConfigureServices(ConfigureServices<MetaballsAppSettings, MainState>);
	}

	private static void ConfigureServices<TAppSettings, TMainState>(HostBuilderContext hostContext, IServiceCollection services)
		where TAppSettings : AppSettings
		where TMainState : GameState
	{
		// Register configuration.
		services.Configure<TAppSettings>(hostContext.Configuration);

		services.AddSingleton(sp => sp.GetRequiredService<IOptions<TAppSettings>>().Value);
		services.AddSingleton<AppSettings>(sp => sp.GetRequiredService<TAppSettings>());
		services.AddSingleton<IOptions<AppSettings>>(sp => sp.GetRequiredService<IOptions<TAppSettings>>());

		services.AddRetroTK();

		// Register game states.
		services.AddTransient<TMainState>();
	}

}