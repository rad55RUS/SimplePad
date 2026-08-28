namespace SimplePad.Settings
{
    /// <summary>
    /// Contains instances of FunctionalSettings and UISettings, providing a centralized way to access application settings.
    /// </summary>
    public class SettingsContainer
    {
        /// <summary>
        /// Gets or sets the app functional settings.
        /// </summary>
        public FunctionalSettings FunctionalSettings { get; set; } = new();

        /// <summary>
        /// Gets or sets the app UI settings.
        /// </summary>
        public UISettings UISettings { get; set; } = new();
    }
}
