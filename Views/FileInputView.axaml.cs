using Avalonia.Controls;
using HeroTableParser.ViewModels;
using System.IO;

namespace HeroTableParser;

/// <summary>
/// Interaction logic for the FileInputView window.
/// This window allows the user to select an Excel file from their device.
/// It manages dialog result handling and binds to the <see cref="FileInputViewModel"/> for file selection logic.
/// </summary>
public partial class FileInputView : Window
{
    /// <summary>
    /// The view model instance providing file selection logic and data binding.
    /// </summary>
    private FileInputViewModel ViewModel { get; }

    /// <summary>
    /// Gets the result of the dialog, which is the selected file path if confirmed, or null if canceled.
    /// </summary>
    public string? DialogResult { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileInputView"/> class.
    /// Sets up the data context and initializes UI components.
    /// </summary>
    public FileInputView()
    {
        InitializeComponent();
        ViewModel = new FileInputViewModel();
        DataContext = ViewModel;
    }

    /// <summary>
    /// Handles the click event for the Cancel button.
    /// Sets the dialog result to null and closes the window.
    /// </summary>
    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DialogResult = null;
        Close();
    }

    /// <summary>
    /// Handles the click event for the "Set Excel Path" button.
    /// If the selected file exists, sets the dialog result to the file path and closes the window.
    /// </summary>
    private void SetExcelPath_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (File.Exists(ViewModel.FilePath))
        {
            DialogResult = ViewModel.FilePath;
        }
        Close();
    }
}