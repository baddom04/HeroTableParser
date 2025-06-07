using Avalonia.Platform.Storage;
using HeroTableParser.Utils;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace HeroTableParser.ViewModels
{
    /// <summary>
    /// ViewModel for handling Excel file input in the UI.
    /// Provides commands and properties for selecting and storing an Excel file reference and its path.
    /// </summary>
    public class FileInputViewModel : ViewModelBase
    {
        /// <summary>
        /// Command to open a file picker dialog for selecting an Excel file.
        /// </summary>
        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }

        private IStorageFile? _excelFile = null;

        /// <summary>
        /// Gets or sets the selected Excel file as an <see cref="IStorageFile"/> instance.
        /// </summary>
        public IStorageFile? ExcelFile
        {
            get => _excelFile;
            set
            {
                this.RaiseAndSetIfChanged(ref _excelFile, value);
            }
        }

        private string _filePath = string.Empty;

        /// <summary>
        /// Gets or sets the local file path of the selected Excel file.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { this.RaiseAndSetIfChanged(ref _filePath, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInputViewModel"/> class.
        /// Sets up the command for opening the file picker.
        /// </summary>
        public FileInputViewModel()
        {
            OpenFileCommand = ReactiveCommand.CreateFromTask(OpenFileAsync);
        }

        /// <summary>
        /// Asynchronously opens a file picker dialog for Excel files.
        /// If a file is selected, updates <see cref="FilePath"/> and <see cref="ExcelFile"/>.
        /// </summary>
        private async Task OpenFileAsync()
        {
            var file = await Picker.DoOpenFilePickerAsync("Excel táblázat", "Excel fájlok", "*.xlsx");
            if (file == null) return;

            FilePath = file.Path.LocalPath;
            ExcelFile = file;
        }
    }
}