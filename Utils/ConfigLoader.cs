using System.IO;

namespace HeroTableParser.Utils
{
    /// <summary>
    /// Handles loading and saving of simple configuration data to a file within the application's options_config directory.
    /// Each instance manages a specific config file, determined by the provided loader ID.
    /// </summary>
    public class ConfigLoader
    {
        /// <summary>
        /// The directory path where configuration files are stored.
        /// </summary>
        private readonly string _directoryPath =
            Path.Combine(ApplicationInfo.AppFolderPath, "options_config");

        /// <summary>
        /// The full file path for the specific configuration file managed by this loader.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigLoader"/> class.
        /// Ensures the configuration directory exists and sets the file path based on the loader ID.
        /// </summary>
        /// <param name="loaderId">A unique identifier for the config file. The string "Loader" is replaced with ".config" and converted to lowercase.</param>
        public ConfigLoader(string loaderId)
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
            _filePath = Path.Combine(
                _directoryPath,
                loaderId.Replace("Loader", ".config").ToLower()
            );
        }

        /// <summary>
        /// Loads the content of the configuration file as a string.
        /// Returns null if the file does not exist.
        /// </summary>
        /// <returns>The file content as a string, or null if the file is missing.</returns>
        public string? Load()
        {
            if (!File.Exists(_filePath)) return null;

            return File.ReadAllText(_filePath);
        }

        /// <summary>
        /// Saves the provided content to the configuration file.
        /// If the content is null, the configuration file is deleted.
        /// </summary>
        /// <param name="content">The content to save, or null to delete the file.</param>
        public void Save(string? content)
        {
            if (content is null)
                File.Delete(_filePath);
            else
                File.WriteAllText(_filePath, content);
        }
    }
}