using System.Windows;
using UserSetting.Controls.CustomUI;
using UserSetting.WPF.Properties;

namespace UserSetting.WPF.CustomUI;

/// <summary>
/// Stores and manages user window preferences such as position, size, and state.
/// </summary>
public class UserPreference
{
    #region Properties

    private double WindowTop { get; set; }
    private double WindowLeft { get; set; }
    private double WindowHeight { get; set; }
    private double WindowWidth { get; set; }
    private WindowState WindowState { get; set; }

    #endregion Properties

    #region Constructor

    public UserPreference()
    {
        Load();
    }

    #endregion Constructor

    #region Load

    /// <summary>
    /// Loads user window preferences from the application settings.
    /// </summary>
    private void Load()
    {
        WindowTop = Settings.Default.WindowTop;
        WindowLeft = Settings.Default.WindowLeft;
        WindowHeight = Settings.Default.WindowHeight;
        WindowWidth = Settings.Default.WindowWidth;
        WindowState = Settings.Default.WindowState;
        CustomWindow.IsMaximized = Settings.Default.IsMaximized; // Optional
    }

    #endregion Load

    #region Save

    /// <summary>
    /// Saves the current window position, size, and state to user settings.
    /// Skips saving if the window is minimized.
    /// </summary>
    public static void Save(Window window)
    {
        if (window.WindowState == WindowState.Minimized)
            return;

        Settings.Default.WindowTop = window.RestoreBounds.Top;
        Settings.Default.WindowLeft = window.RestoreBounds.Left;
        Settings.Default.WindowHeight = window.RestoreBounds.Height;
        Settings.Default.WindowWidth = window.RestoreBounds.Width;
        Settings.Default.WindowState = window.WindowState;
        Settings.Default.IsMaximized = window.WindowState == WindowState.Maximized; // Optional
        CustomWindow.IsMaximized = Settings.Default.IsMaximized; // Optional
        Settings.Default.MaximumButtonText = CustomWindow.IsMaximized ? "2" : "1"; // Optional
        Settings.Default.Save();
    }

    #endregion Save

    #region Apply

    /// <summary>
    /// Region containing methods to apply stored user preferences to a window.
    /// </summary>
    public void Apply(Window window)
    {
        if (WindowWidth > 0 && WindowHeight > 0)
        {
            window.Top = WindowTop;
            window.Left = WindowLeft;
            window.Height = WindowHeight;
            window.Width = WindowWidth;
            window.WindowState = WindowState;
        }
        else
        {
            CenterWindow(window);
        }
        
#if NET5_0_OR_GREATER

        // Only move into view if there is a single monitor
        if (AreAlmostEqual(SystemParameters.VirtualScreenWidth, SystemParameters.PrimaryScreenWidth) &&
            AreAlmostEqual(SystemParameters.VirtualScreenHeight, SystemParameters.PrimaryScreenHeight))
        {
            MoveIntoView(window);
        }
#else
        // Only move into view if there is a single monitor
        // using System.Windows.Forms; // <== Add this at the top
        if (Screen.AllScreens.Length == 1)
        {
            MoveIntoView(window);
        }
#endif
    }

    #endregion Apply
    
    #region AreAlmostEqual
    
    /// <summary>
    /// Determines whether two double values are almost equal, within a small epsilon.
    /// </summary>
    /// <param name="a">The first double value.</param>
    /// <param name="b">The second double value.</param>
    /// <returns>True if the values are almost equal; otherwise, false.</returns>
    private static bool AreAlmostEqual(double a, double b)
    {
        // Define a small epsilon for floating-point comparison
       const double epsilon = 1e-6;
        
        return Math.Abs(a - b) < epsilon;
    }
    
    #endregion AreAlmostEqual

    #region MoveIntoView

    /// <summary>
    /// Ensures the specified window is fully visible within the virtual screen bounds.
    /// Adjusts the window position if it extends beyond the screen edges.
    /// </summary>
    private static void MoveIntoView(Window window)
    {
        var screenHeight = SystemParameters.VirtualScreenHeight;
        var screenWidth = SystemParameters.VirtualScreenWidth;

        if (window.Top + window.Height > screenHeight)
            window.Top = screenHeight - window.Height;
        if (window.Left + window.Width > screenWidth)
            window.Left = screenWidth - window.Width;
        if (window.Top < 0)
            window.Top = 0;
        if (window.Left < 0)
            window.Left = 0;
    }

    #endregion MoveIntoView

    #region CenterWindow

    /// <summary>
    /// Centers the specified window on the primary screen if no valid size stored.
    /// </summary>
    private static void CenterWindow(Window window)
    {
        var screenHeight = SystemParameters.PrimaryScreenHeight;
        var screenWidth = SystemParameters.PrimaryScreenWidth;

        window.Top = (screenHeight - window.Height) / 2;
        window.Left = (screenWidth - window.Width) / 2;
    }

    #endregion CenterWindow
}