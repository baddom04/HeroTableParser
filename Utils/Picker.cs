using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System;
using System.Threading.Tasks;

namespace HeroTableParser.Utils
{
    /// <summary>
    /// Provides static helper methods for displaying file and folder picker dialogs in an Avalonia application.
    /// Utilizes the application's main window storage provider to open file and folder selection dialogs asynchronously.
    /// </summary>
    public static class Picker
    {
        /// <summary>
        /// Cached reference to the application's storage provider.
        /// </summary>
        private static IStorageProvider? _provider;

        /// <summary>
        /// Retrieves the <see cref="IStorageProvider"/> from the application's main window.
        /// Throws an exception if the provider is not available.
        /// </summary>
        /// <returns>The storage provider for file and folder dialogs.</returns>
        /// <exception cref="NullReferenceException">Thrown if the storage provider cannot be found.</exception>
        private static IStorageProvider GetProvider()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.StorageProvider is not { } provider)
            {
                throw new NullReferenceException("Missing StorageProvider instance.");
            }

            return provider;
        }

        /// <summary>
        /// Opens a file picker dialog asynchronously, allowing the user to select a single file.
        /// </summary>
        /// <param name="title">The title of the file picker dialog.</param>
        /// <param name="fileType">The description of the file type filter (e.g., "Excel Files").</param>
        /// <param name="patterns">The file extension patterns to filter (e.g., "*.xlsx").</param>
        /// <returns>The selected <see cref="IStorageFile"/> if a file is chosen; otherwise, null.</returns>
        public static async Task<IStorageFile?> DoOpenFilePickerAsync(string title, string? fileType, params string[] patterns)
        {
            _provider ??= GetProvider();

            var options = new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = fileType is not null ? [new FilePickerFileType(fileType) { Patterns = patterns }] : []
            };

            var files = await _provider.OpenFilePickerAsync(options);

            return files?.Count >= 1 ? files[0] : null;
        }

        /// <summary>
        /// Opens a folder picker dialog asynchronously, allowing the user to select a single folder.
        /// </summary>
        /// <param name="title">The title of the folder picker dialog.</param>
        /// <returns>The selected <see cref="IStorageFolder"/> if a folder is chosen; otherwise, null.</returns>
        public static async Task<IStorageFolder?> DoOpenFolderPickerAsync(string title)
        {
            _provider ??= GetProvider();

            var options = new FolderPickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
            };

            var directories = await _provider.OpenFolderPickerAsync(options);

            return directories?.Count >= 1 ? directories[0] : null;
        }
    }
}