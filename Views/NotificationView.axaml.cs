using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using HeroTableParser.Views;
using System.Threading.Tasks;

namespace HeroTableParser;

/// <summary>
/// Interaction logic for the NotificationView window.
/// Displays a notification dialog with a subject, message, notification type, and optional additional information.
/// Provides static method for showing notifications as modal dialogs.
/// </summary>
public partial class NotificationView : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationView"/> class.
    /// Sets up the notification content and visual style based on the notification type.
    /// </summary>
    /// <param name="subject">The subject or title of the notification.</param>
    /// <param name="message">The main message to display.</param>
    /// <param name="type">The type of notification (Error, Warning, Success, etc.).</param>
    /// <param name="addInfo">Optional additional information to display.</param>
    public NotificationView(string subject, string message, NotificationType type, string? addInfo = null)
    {
        InitializeComponent();
        Subject.Text = subject;
        Message.Text = message;
        SetSymbol(type);

        if (addInfo == null)
            AdditionalInfo.IsVisible = false;
        else
            AdditionalInfo.Text = addInfo;
    }

    /// <summary>
    /// Handles the click event for the OK button, closing the notification dialog.
    /// </summary>
    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Sets the icon and background color for the notification based on its type.
    /// </summary>
    /// <param name="type">The type of notification.</param>
    private void SetSymbol(NotificationType type)
    {
        Application.Current!.TryGetResource(type.ToString().ToLower(), out var symbol);
        Symbol.Data = (Geometry)symbol!;
        switch (type)
        {
            case NotificationType.Error:
                SymbolBG.Classes.Add("redBG");
                break;
            case NotificationType.Warning:
                SymbolBG.Classes.Add("yellowBG");
                break;
            case NotificationType.Success:
                SymbolBG.Classes.Add("greenBG");
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Displays a modal notification dialog with the specified content and type.
    /// Waits for the main window to be available before showing the dialog.
    /// </summary>
    /// <param name="titleKey">The subject or title of the notification.</param>
    /// <param name="messageKey">The main message to display.</param>
    /// <param name="type">The type of notification.</param>
    /// <param name="addInfo">Optional additional information to display.</param>
    public static async void Notify(string titleKey, string messageKey, NotificationType type, string? addInfo = null)
    {
        var mw = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        while (mw == null || mw is not MainWindow)
        {
            await Task.Delay(1000);
            mw = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        }

        var notificationDialog = new NotificationView(titleKey, messageKey, type, addInfo);
        await notificationDialog.ShowDialog((MainWindow)mw);
    }
}