using System.ComponentModel;
using System.Windows;
using UserSetting.Controls.CustomUI;
using UserSetting.WPF.Properties;

namespace UserSetting.WPF.CustomUI;

public class StanWindow : CustomWindow
{
    #region Fields
    
    /// <summary>
    /// Stores user preference settings for the window.
    /// </summary>
    private readonly UserPreference _userPreference;

    /// <summary>
    /// Identifies the <see cref="Subtitle"/> dependency property.
    /// Used to store the subtitle text for the window.
    /// </summary>
    private static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(StanWindow),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="IsTestEnvironment"/> dependency property.
    /// Used to indicate if the application is running in a test or development environment.
    /// </summary>
    private static readonly DependencyProperty IsTestEnvironmentProperty =
        DependencyProperty.Register(nameof(IsTestEnvironment), typeof(bool), typeof(StanWindow));

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets or sets the subtitle text for the window.
    /// </summary>
    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <remarks>
    /// This property is used to determine if the application is running in a development or test environment.
    /// </remarks>
    /// <reference>
    /// https://stackoverflow.com/a/7335737/5352166/>
    /// </reference>
    public bool IsTestEnvironment
    {
        get => (bool)GetValue(IsTestEnvironmentProperty);
        set => SetValue(IsTestEnvironmentProperty, value);
    }

    #endregion Properties
    
    #region Constructor
    
    protected StanWindow()
    {
        //IsTestEnvironment = Defines.Environment == "Dev" || Defines.Environment == "Test" || Defines.Environment == "Test ReadOnly" || Defines.Environment == "Live ReadOnly";
        //Subtitle = Defines.Environment;
        
        _userPreference = new UserPreference();
        Loaded += StanWindowLoaded;
        Closing += StanWindowClosing;
    }

    #endregion Constructor
    
    #region StanWindowLoaded

    /// <summary>
    /// Handles the Loaded event for the StanWindow.
    /// Initializes window state and applies user preferences.
    /// </summary>
    private void StanWindowLoaded(object sender, RoutedEventArgs e)
    {
        IsMaximized = Settings.Default.IsMaximized; // Optional
        MaximumButtonText = Settings.Default.MaximumButtonText; // Optional
        _userPreference.Apply(this);
    }
    
    #endregion StanWindowLoaded
    
    #region StanWindowClosing

    /// <summary>
    /// Handles the Closing event for the StanWindow.
    /// Saves the current user preferences before the window is closed.
    /// </summary>
    private void StanWindowClosing(object? sender, CancelEventArgs e)
    {
        UserPreference.Save(this);
    }
    
    #endregion StanWindowClosing
}
