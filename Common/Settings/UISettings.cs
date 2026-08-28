namespace SimplePad.Settings
{
    /// <summary>
    /// Contains settings related to the user interface, such as window position and size, allowing for customization of the application's appearance and layout.
    /// </summary>
    public class UISettings
    {
        /// <summary>
        /// Gets or sets the X coordinate of the main window's position on the screen.
        /// <br/><br/>
        /// A value of -1 indicates that the position is not set and the window will be centered on the screen.
        /// </summary>
        public int MainWindowX { get; set; } = -1;

        /// <summary>
        /// Gets or sets the Y coordinate of the main window's position on the screen. 
        /// <br/><br/>
        /// A value of -1 indicates that the position is not set and the window will be centered on the screen.
        /// </summary>
        public int MainWindowY { get; set; } = -1;

        /// <summary>
        /// Gets or sets the width of the main window.
        /// </summary>
        public double MainWindowWidth { get; set; } = 1280;

        /// <summary>
        /// Gets or sets the height of the main window.
        /// </summary>
        public double MainWindowHeight { get; set; } = 720;
    }
}
