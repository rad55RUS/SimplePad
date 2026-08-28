using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SimplePad.Services;
using SimplePad.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplePad
{
    /// <summary>
    /// Contains the entry point of the application and methods for building the host and initializing user settings.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// Builds the host for the application, binding default settings to the configuration and initializing user settings.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHost BuildHost(string[] args)
        {
            HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

            // Bind default settings to the configuration
            hostBuilder.Configuration.GetSection(nameof(FunctionalSettings)).Bind(SettingsService.DefaultSettings.FunctionalSettings);
            hostBuilder.Configuration.GetSection(nameof(UISettings)).Bind(SettingsService.DefaultSettings.UISettings);
            //

            // Bind default settings to user settings
            SettingsService.UserSettings.FunctionalSettings = SettingsService.DefaultSettings.FunctionalSettings;
            SettingsService.UserSettings.UISettings = SettingsService.DefaultSettings.UISettings;
            //

            var host = hostBuilder.Build();

            InitializeUserSettings();

            return host;
        }

        /// <summary>
        /// Initialization code. Don't use any Avalonia, third-party APIs or any
        /// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        /// yet and stuff might break.
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        public static void Main(string[] args)
        {
            App.Args = args;

            IHost host = BuildHost(args);
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        /// <summary>
        /// Avalonia configuration, don't remove; also used by visual designer.
        /// </summary>
        /// <returns></returns>
        public static AppBuilder BuildAvaloniaApp()
        {
            AppBuilder appBuilder = AppBuilder.Configure<App>();
            appBuilder.UsePlatformDetect();
            appBuilder.WithInterFont();
            appBuilder.LogToTrace();

            return appBuilder;
        }

        /// <summary>
        /// Initializes user settings by loading them from a JSON file in the user's Documents folder, merging them with default settings, and binding them to the application's configuration.
        /// </summary>
        private static void InitializeUserSettings()
        {
            var userConfigBuilder = new ConfigurationBuilder();

            var defaultValues = new Dictionary<string, string?>();

            // Add default settings to the dictionary
            Common.AddPropertiesToDictionary(defaultValues, "FunctionalSettings", SettingsService.DefaultSettings.FunctionalSettings);
            Common.AddPropertiesToDictionary(defaultValues, "UISettings", SettingsService.DefaultSettings.UISettings);
            //

            // Add default values to the configuration builder and load user settings from a JSON file
            userConfigBuilder.AddInMemoryCollection(defaultValues);
            userConfigBuilder.AddJsonFile($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\SimplePad\\usersettings.json", optional: true, reloadOnChange: true);
            //

            var userConfig = userConfigBuilder.Build();

            // Bind user settings to the application's configuration
            userConfig.GetSection(nameof(FunctionalSettings)).Bind(SettingsService.UserSettings.FunctionalSettings);
            userConfig.GetSection(nameof(UISettings)).Bind(SettingsService.UserSettings.UISettings);
            //
        }
    }
}