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
    /// 
    /// </summary>
    public class FileDialogService
    {
        #region Open file methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>?> OpenFileAsync()
        {
            return await OpenFileAsync(true);
        }

        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static async Task<List<string>?> OpenFileAsync(string extension, string desc)
        {
            return await OpenFileAsync(extension, desc, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
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
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static async Task<List<string>?> OpenFileAsync(FilePickerOpenOptions options)
        {
            var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var storage = desktop?.MainWindow?.StorageProvider;

            if (storage == null) return null;

            var files = await storage.OpenFilePickerAsync(options);

            return [.. files.Select(f => f.Path.LocalPath)];
        }
        #endregion

        #region Open folder methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>?> OpenFolderAsync()
        {
            FolderPickerOpenOptions options = new()
            {
                AllowMultiple = true,
            };

            return await OpenFolderAsync(options);
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
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task<List<string>?> OpenFolderAsync(FolderPickerOpenOptions options)
        {
            var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var storage = desktop?.MainWindow?.StorageProvider;

            if (storage == null) return null;

            var folders = await storage.OpenFolderPickerAsync(options);

            return [.. folders.Select(f => f.Path.LocalPath)];
        }
        #endregion

        #region Save file methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<string?> SaveFileAsync()
        {
            FilePickerSaveOptions options = new();

            return await SaveFileAsync(options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static async Task<string?> SaveFileAsync(FilePickerSaveOptions options)
        {
            var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var storage = desktop?.MainWindow?.StorageProvider;

            if (storage == null) return null;

            var file = await storage.SaveFilePickerAsync(options);

            return file?.Path.LocalPath;
        }
        #endregion
    }
}