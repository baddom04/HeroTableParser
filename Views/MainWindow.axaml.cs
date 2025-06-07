using Avalonia.Controls;
using HeroTableParser.ViewModels;

namespace HeroTableParser.Views
{
    /// <summary>
    /// Code-behind for the main application window.
    /// Handles initialization, Excel file path selection, and user-triggered path updates.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The main view model for this window, providing data and logic for hero selection and analysis.
        /// </summary>
        public MainWindowViewModel ViewModel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Sets up the data context and attaches the Loaded event handler.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;

            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Handles the window's Loaded event.
        /// If no Excel path is set, prompts the user to select a file and closes the window if canceled.
        /// </summary>
        private void MainWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ViewModel.AppInfo.ExcelPath is not null) return;

            GetAndSetPath(true);
        }

        /// <summary>
        /// Opens the file input dialog for selecting an Excel file.
        /// If the user cancels and <paramref name="closeOnCancel"/> is true, closes the main window.
        /// Otherwise, sets the selected path and re-initializes the view model.
        /// </summary>
        /// <param name="closeOnCancel">Whether to close the window if the dialog is canceled.</param>
        private async void GetAndSetPath(bool closeOnCancel)
        {
            FileInputView fileInputView = new();
            await fileInputView.ShowDialog(this);

            if (fileInputView.DialogResult is null && closeOnCancel)
            {
                Close();
                return;
            }
            else if (fileInputView.DialogResult is null) return;

            ViewModel.AppInfo.ExcelPath = fileInputView.DialogResult;
            ViewModel.Init();
        }

        /// <summary>
        /// Handles the click event for the "Path update" button.
        /// Opens the file input dialog without closing the main window if canceled.
        /// </summary>
        private void NewPath_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            GetAndSetPath(false);
        }
    }
}