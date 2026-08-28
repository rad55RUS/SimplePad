using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;


namespace SimplePad.Services
{
    /// <summary>
    /// Provides methods for opening file and folder dialogs, as well as saving files, using Avalonia's storage provider.
    /// </summary>
    public static class FileDialogueService
    {
        #region Open file methods
        /// <summary>
        /// Opens <b>dialog</b> that allows the user to select multiple <b>files</b>
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>?> OpenFileAsync()
        {
            return await OpenFileAsync(true);
        }

        /// <summary>
        /// Opens <b>dialog</b> that allows the user to select one or more <b>files</b>
        /// </summary>
        /// <param name="isMultiple">Whether to allow selecting <b>multiple files</b> in the <b>dialog</b></param>
        /// <returns></returns>
        public static async Task<List<string>?> OpenFileAsync(bool isMultiple)
        {
            FilePickerOpenOptions options = new()
            {
                AllowMultiple = isMultiple,
            };

            return await OpenFileAsync(options);
        }

        /// <summary>
        /// Opens <b>dialog</b> that allows the user to select <b>multiple files</b>
        /// </summary>
        /// <param name="extension">
        /// The <b>file extension</b> (without the dot) to filter the <b>files shown</b> in the <b>dialog</b>
        /// <br/><br/>
        /// <i>e.g. "txt" for text files, "jpg" for JPEG image files, etc</i>
        /// </param>
        /// <param name="desc">The description of <b>file extension</b> to show in the <b>dialog</b></param>
        /// <returns></returns>
        public static async Task<List<string>?> OpenFileAsync(string extension, string desc)
        {
            return await OpenFileAsync(extension, desc, true);
        }

        /// <summary>
        /// Opens <b>dialog</b> that allows the user to select one or more <b>files</b>
        /// </summary>
        /// <param name="extension">
        /// The <b>file extension</b> (without the dot) to filter the <b>files shown</b> in the <b>dialog</b>
        /// <br/><br/>
        /// <i>e.g. "txt" for text files, "jpg" for JPEG image files, etc</i>
        /// </param>
        /// <param name="desc">The description of <b>file extension</b> to show in the <b>dialog</b></param>
        /// <param name="isMultiple">Whether to allow selecting <b>multiple files</b> in the <b>dialog</b></param>
        /// <returns></returns>
        public static async Task<List<string>?> OpenFileAsync(string extension, string desc, bool isMultiple)
        {
            FilePickerFileType type = new(desc)
            {
                Patterns = [$"*.{extension}"],
            };

            FilePickerOpenOptions options = new()
            {
                AllowMultiple = isMultiple,
                FileTypeFilter = [type]
            };

            return await OpenFileAsync(options);
        }

        /// <summary>
        /// Opens <b>dialog</b> that allows the user to select one or more <b>files</b> based on the provided <b>options</b>
        /// </summary>
        /// <param name="options">The <see cref="FilePickerOpenOptions"/> that specifies the <b>configuration</b> for the <b>dialog</b>, such as whether to allow selecting multiple files</param>
        /// <returns></returns>
        private static async Task<List<string>?> OpenFileAsync(FilePickerOpenOptions options)
        {
            var storage = GetMainWindowStorageProvider();
            var files = await storage.OpenFilePickerAsync(options);

            return [.. files.Select(f => f.Path.LocalPath)];
        }
        #endregion

        #region Open folder methods
        /// <summary>
        /// Opens <b>dialog</b> that allows the user to select multiple <b>folders</b>
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>?> OpenFolderAsync()
        {
            return await OpenFolderAsync(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>?> OpenFolderAsync(bool isMultiple)
        {
            FolderPickerOpenOptions options = new()
            {
                AllowMultiple = isMultiple,
            };

            return await OpenFolderAsync(options);
        }

        /// <summary>
        /// Opens <b>dialog</b> that allows the user to select one or more <b>folders</b> based on the provided <b>options</b>
        /// </summary>
        /// <param name="options">The <see cref="FolderPickerOpenOptions"/> that specifies the <b>configuration</b> for the <b>dialog</b>, such as whether to allow selecting multiple folders</param>
        /// <returns></returns>
        private static async Task<List<string>?> OpenFolderAsync(FolderPickerOpenOptions options)
        {
            var storage = GetMainWindowStorageProvider();
            var folders = await storage.OpenFolderPickerAsync(options);

            return [.. folders.Select(f => f.Path.LocalPath)];
        }
        #endregion

        #region Save file methods
        /// <summary>
        /// Opens <b>dialog</b> that allows the user to select <b>file saving locaion</b>
        /// </summary>
        /// <returns></returns>
        public static async Task<string?> SaveFileAsync()
        {
            FilePickerSaveOptions options = new()
            {
                SuggestedFileName = "TextFile.txt",
            };

            return await SaveFileAsync(options);
        }

        /// <summary>
        /// Opens <b>dialog</b> that allows the user to select <b>file saving locaion</b> based on the provided <b>options</b>
        /// </summary>
        /// <param name="options">The <see cref="FilePickerSaveOptions"/> that specifies the <b>configuration</b> for the <b>dialog</b>, such as the default file name, file extension filter, etc</param>
        /// <returns></returns>
        private static async Task<string?> SaveFileAsync(FilePickerSaveOptions options)
        {
            var storage = GetMainWindowStorageProvider();
            var file = await storage.SaveFilePickerAsync(options);

            return file?.Path.LocalPath;
        }
        #endregion

        /// <summary>
        /// Gets the storage provider associated with the main window of the application.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when the MainWindow storage provider is not available</exception>
        private static IStorageProvider GetMainWindowStorageProvider()
        {
            var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            return desktop?.MainWindow?.StorageProvider ?? throw new InvalidOperationException("MainWindow storage provider is not available");
        }
    }
}