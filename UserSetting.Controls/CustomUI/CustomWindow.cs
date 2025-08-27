using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserSetting.Controls.Properties;

namespace UserSetting.Controls.CustomUI;

/// <summary>
///
///  Ref: https://stackoverflow.com/a/35171803/5352166
///
/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
///
/// Step 1a) Using this custom control in a XAML file that exists in the current project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is
/// to be used:
///
///     xmlns:cw="clr-namespace:SWIM.Controls.CustomUI"
///
///
/// Step 1b) Using this custom control in a XAML file that exists in a different project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is
/// to be used:
///
///     xmlns:cw="clr-namespace:SWIM.Controls.CustomUI;assembly=SWIM.Controls.CustomUI"
///
/// You will also need to add a project reference from the project where the XAML file lives
/// to this project and Rebuild to avoid compilation errors:
///
///     Right-click on the target project in the Solution Explorer and
///     "Add Reference"->"Projects"->[Select this project]
///
///
/// Step 2)
/// Go ahead and use your control in the XAML file.
///
///     <cw:CustomControl1/>
///
/// </summary>
public class CustomWindow : MaximizeWindowBase, INotifyPropertyChanged
{
    protected CustomWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));

        // StartupWindowMaximized();
    }

    #region Fields

    /// <summary>
    /// Initialize the maximum button text to show the "WindowState"
    /// </summary>
    private string? _maximumButtonText = "1";

    /// <summary>
    /// Get the entry assembly of the application
    /// </summary>
    /// <returns></returns>
    private static readonly CoreAssemblyInfo EntryAssembly = new(Assembly.GetEntryAssembly()!);

    /// <summary>
    /// Get the author of the application
    /// </summary>
    private static readonly string Author = EntryAssembly.Author;

    /// <summary>
    /// Get the author department
    /// </summary>
    private static readonly string Department = EntryAssembly.Department;

    /// <summary>
    /// Get the application copy right
    /// </summary>
    private static readonly string Copyright = EntryAssembly.Copyright;

    /// <summary>
    /// Get the version of the application
    /// </summary>
    private static readonly string AppVersion = EntryAssembly.Version;

    /// <summary>
    /// Get the framework version of the application
    /// </summary>
    private static readonly string FrameworkVersion = RuntimeInformation.FrameworkDescription;

    /// <summary>
    /// Set the author credits property
    /// </summary>
    /// <param name="{version}"></param>
    /// <returns></returns>
    private static readonly DependencyProperty AuthorCreditsProperty =
        DependencyProperty.Register("AuthorCredits", typeof(string), typeof(CustomWindow),
            new PropertyMetadata($"{Copyright} | {Author} | {Department} | {FrameworkVersion} | {AppVersion}"));

    #endregion Fields

    #region Properties

    /// <summary>
    /// This property initiates an object to be shown on the custom window status bar
    /// </summary>
    public object AuthorCredits => (string)GetValue(AuthorCreditsProperty);

    /// <summary>
    /// Indicates whether the custom window is currently maximized.
    /// </summary>
    public static bool IsMaximized { get; set; }

    /// <summary>
    /// Get or Set the maximum button text
    /// </summary>
    /// <value></value>
    public string? MaximumButtonText
    {
        get => _maximumButtonText;
        set
        {
            _maximumButtonText = value;
            OnPropertyChanged(nameof(MaximumButtonText));
        }
    }

    #endregion Properties

    #region Custom Window Controls

    #region MinimizeEventHandler

    /// <summary>
    /// Minimizing the active custom window is fired by this method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MinimizeEventHandler(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
        OnPropertyChanged(nameof(MaximumButtonText));
    }

    #endregion MinimizeEventHandler

    #region MaximumEventHandler

    /// <summary>
    /// This method provides the event handler for the custom control
    /// responsible for maximizing or restoring the active window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MaximumEventHandler(object sender, RoutedEventArgs e)
    {
        // Maximize or normalize the custom window
        Maximize();
    }

    #endregion MaximumEventHandler

    #region CloseEventHandler

    /// <summary>
    /// The method provides a means to shut down the window on demand
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void CloseEventHandler(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    #endregion CloseEventHandler

    #region CustomTitleBarEventHandler

    /// <summary>
    /// This method the event handlers responsible for moving the custom window to new positions and changing the window state
    /// Inspired by this link: https://learn.microsoft.com/en-us/answers/questions/572460/how-to-create-custom-window-style-in-wpf-(drag-win
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="mouseEvent"></param>
    private void CustomTitleBarEventHandler(object sender, MouseButtonEventArgs mouseEvent)
    {
        // The condition to restore or normalize the custom window if moved
        var restoreIfMoved = false;

        // Cast the sender to Grid and set it as the header
        var header = (Grid)sender;

        if (mouseEvent is { LeftButton: MouseButtonState.Pressed, ClickCount: 2 })
        {
            // Maximize or normalize the custom window
            Maximize();
        }
        else
        {
            // Set the condition to restore or normalize the custom window if moved to true
            if (mouseEvent.ChangedButton == MouseButton.Left && WindowState == WindowState.Maximized)
            {
                restoreIfMoved = true;
            }

            // Drag and move the custom window
            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                // Drag and move the custom window
                DragMove();
            }
        }

        header.MouseLeftButtonUp += (_, _) =>
        {
            // Set the condition to restore or normalize the custom window
            // if moved to false when the left button is released
            restoreIfMoved = false;
        };

        header.MouseMove += (_, e) =>
        {
            if (!restoreIfMoved)
            {
                return;
            }

            // Since this is true, we first have to set it to false
            restoreIfMoved = false;

            // Move the custom window to the new position
            MoveAndRestoreWindow(e);

            _maximumButtonText = "1";
            OnPropertyChanged(nameof(MaximumButtonText));
        };

        _maximumButtonText = WindowState == WindowState.Maximized || IsMaximized ? "2" : "1";
        OnPropertyChanged(nameof(MaximumButtonText));
    }

    #endregion CustomTitleBarEventHandler

    #region MoveAndRestoreWindow

    /// <summary>
    /// Moves and restores a window to normal position when it is in a maximized state.
    /// </summary>
    /// <param name="e">The MouseEventArgs object that contains information about the mouse event.</param>
    private void MoveAndRestoreWindow(MouseEventArgs e)
    {
        if
        (
            e.RightButton == MouseButtonState.Pressed ||
            e.MiddleButton == MouseButtonState.Pressed ||
            e.LeftButton != MouseButtonState.Pressed ||
            WindowState != WindowState.Maximized ||
            ResizeMode == ResizeMode.NoResize
        ) return;
        
        // Calculating correct left coordinate for multiscreen system.
        var mouseAbsolute = PointToScreen(Mouse.GetPosition(this));
        var width = RestoreBounds.Width;
        var left = mouseAbsolute.X - width / 2;

        // Aligning window's position to fit the screen.
        var virtualScreenWidth = SystemParameters.VirtualScreenWidth;
        left = left + width > virtualScreenWidth ? virtualScreenWidth - width : left;

        var mousePosition = e.MouseDevice.GetPosition(this);

        // When dragging the window down at the very top of the border,
        // move the window a bit upwards to avoid showing the resize handle as soon as the mouse button is released
        Top = mousePosition.Y < 5 ? -5 : mouseAbsolute.Y - mousePosition.Y;
        Left = left;

        // Restore window to normal state.
        WindowState = WindowState.Normal;

        DragMove();
    }

    #endregion MoveAndRestoreWindow

    #region Maximize

    /// <summary>
    /// A method to maximize or normalize the custom window
    /// </summary>
    private void Maximize()
    {
        switch (WindowState)
        {
            case WindowState.Normal:
                WindowState = WindowState.Maximized;
                IsMaximized = true;
                _maximumButtonText = "2";
                OnPropertyChanged(nameof(MaximumButtonText));
                break;
            default:
                WindowState = WindowState.Normal;
                IsMaximized = false;
                _maximumButtonText = "1";
                OnPropertyChanged(nameof(MaximumButtonText));
                break;
        }
    }

    #endregion Maximize

    #region StartupWindowMaximized Moved Logic To StanWindow

    /*/// <summary>
    /// A method to start the custom window in a maximized state
    /// </summary>
    private void StartupWindowMaximized()
    {
        WindowState = WindowState.Maximized;
        MaximumButtonText = "2";
        IsMaximized = true;
    }*/

    #endregion StartupWindowMaximized Moved Logic To StanWindow

    #endregion Custom Window Controls

    #region Apply Custom Controls On Implemented Windows

    /// <summary>
    /// This method provides the template for creating the event handlers
    /// </summary>
    public override void OnApplyTemplate()
    {

        if (GetTemplateChild("MinimizeButton") is Button minimizeButton)
            minimizeButton.Click += MinimizeEventHandler;

        if (GetTemplateChild("MaximumButton") is Button maximumButton)
            maximumButton.Click += MaximumEventHandler;

        if (GetTemplateChild("CloseButton") is Button closeButton)
            closeButton.Click += CloseEventHandler;

        if (GetTemplateChild("CustomTitleBar") is Grid windowChrome)
            windowChrome.MouseDown += CustomTitleBarEventHandler;

        base.OnApplyTemplate();
    }

    #endregion Apply Custom Controls On Implemented Windows

    #region PropertyChanged

    /// <summary>
    /// An event handler to notify the custom window of any property changes
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// A method to notify the custom window of any property changes
    /// </summary>
    /// <param name="propertyName"></param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion PropertyChanged
}
