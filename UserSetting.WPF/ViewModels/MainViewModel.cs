using System.Collections.ObjectModel;
using System.Windows.Input;
using UserSetting.WPF.Commands;
using UserSetting.WPF.CustomUI;
using UserSetting.WPF.Models;
using UserSetting.WPF.Properties;

namespace UserSetting.WPF.ViewModels;

internal class MainViewModel : ViewModelBase
{
    #region Fields

    private const string BaseUri = "pack://application:,,,/UserSetting.Controls;component/Themes";

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the collection of languages
    /// </summary>
    public ObservableCollection<UiTheme> MenuItems { get; private set; }

    /// <summary>
    /// Gets the command to change the theme.
    /// </summary>
    public static ICommand ThemeChangeCommand => new ActionCommand(p => SetUiTheme(p.ToString()));

    #endregion Properties

    #region MainViewModel

    public MainViewModel()
    {
        MenuItems = [];

        GetUiThemes();
        //_shellViewModel?.SetPanelTheme();
    }

    #endregion MainViewModel

    #region GetThemes

    /// <summary>
    /// Retrieves the available themes and assigns them to the menu items.
    /// </summary>
    private void GetUiThemes()
    {
        MenuItems =
        [
            new UiTheme { Header = "Blue"     , Color = "#0078D7", Uri = $"{BaseUri}/BlueTheme.xaml" },
            new UiTheme { Header = "LightDark", Color = "#464646", Uri = $"{BaseUri}/LightDarkTheme.xaml" },
            new UiTheme { Header = "Dark"     , Color = "#000000", Uri = $"{BaseUri}/DarkTheme.xaml" },
            new UiTheme { Header = "Green"    , Color = "#107C10", Uri = $"{BaseUri}/GreenTheme.xaml" },
            new UiTheme { Header = "Purple"   , Color = "#444791", Uri = $"{BaseUri}/PurpleTheme.xaml" }
        ];
    }

    #endregion GetThemes

    #region SetUiTheme

    /// <summary>
    /// Sets the theme of the application based on the specified color name.
    /// This method also changes the anchorable panel theme accordingly.
    /// </summary>
    /// <param name="colorName">The name of the color theme to set.</param>
    private static void SetUiTheme(string? colorName)
    {
        AppTheme.ChangeTheme(new Uri($"{BaseUri}/{colorName}Theme.xaml"));
        Settings.Default.Theme = colorName;

        // save as the default setting
        Settings.Default.Save();

        // change the anchor-able panel theme
        //_shellViewModel?.SetPanelTheme();

    }

    #endregion SetUiTheme
}
