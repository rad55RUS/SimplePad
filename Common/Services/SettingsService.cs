using SimplePad.Settings;
using System;
using System.Diagnostics;
using System.IO;


namespace SimplePad.Services
{
    /// <summary>
    /// Сontains methods and properties for managing application settings, including default and user-specific settings, and saving user settings to a file.
    /// </summary>
    public static class SettingsService
    {
        /// <summary>
        /// Gets or sets the default settings for the application, which are used as a baseline for user-specific settings.
        /// </summary>
        public static SettingsContainer DefaultSettings { get; set; } = new();

        /// <summary>
        /// Gets or sets the user-specific settings for the application, which can be modified and saved by the user.
        /// </summary>
        public static SettingsContainer UserSettings { get; set; } = new();

        /// <summary>
        /// Saves the user-specific settings to a JSON file in the user's Documents folder, creating the necessary directory if it does not exist.
        /// </summary>
        public static void SaveSettings()
        {
            string userSettingsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\SimplePad\\usersettings.json";
            string jsonString = JsonCommon.SerializeJson(UserSettings);

            if (!Directory.Exists(Path.GetDirectoryName(userSettingsPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(userSettingsPath) ?? "");
            }

            File.WriteAllText(userSettingsPath, jsonString);
        }
    }
}