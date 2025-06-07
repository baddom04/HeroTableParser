using System;
using System.IO;

namespace HeroTableParser.Utils
{
    /// <summary>
    /// Provides application-level information and manages the storage and retrieval of the Excel file path.
    /// Handles persistence using <see cref="ConfigLoader"/> and notifies listeners when the Excel path changes.
    /// </summary>
    public class ApplicationInfo
    {
        #region Fields

        /// <summary>
        /// Loader responsible for saving and loading the Excel file path configuration.
        /// </summary>
        private readonly ConfigLoader _configLoader;

        /// <summary>
        /// Backing field for the <see cref="ExcelPath"/> property.
        /// </summary>
        private string? _excelPath;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the application-specific folder path in the user's AppData directory.
        /// Used for storing configuration and other persistent data.
        /// </summary>
        public readonly static string AppFolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HeroTableParser");

        /// <summary>
        /// Gets or sets the path to the Excel file used by the application.
        /// When set, the value is persisted and the <see cref="ExcelPathChanged"/> event is raised.
        /// </summary>
        public string? ExcelPath
        {
            get { return _excelPath; }
            set
            {
                _excelPath = value;
                _configLoader.Save(value);
                ExcelPathChanged?.Invoke();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event triggered whenever the <see cref="ExcelPath"/> property changes.
        /// </summary>
        public event Action? ExcelPathChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor. Ensures the application folder exists in AppData.
        /// </summary>
        static ApplicationInfo()
        {
            if (!Directory.Exists(AppFolderPath))
                Directory.CreateDirectory(AppFolderPath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInfo"/> class.
        /// Loads the Excel file path from persistent storage.
        /// </summary>
        public ApplicationInfo()
        {
            _configLoader = new ConfigLoader("ExcelPathLoader");
            _excelPath = _configLoader.Load();
        }

        #endregion
    }
}