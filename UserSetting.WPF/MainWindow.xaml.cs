using System.ComponentModel;
using System.Windows;
using UserSetting.WPF.CustomUI;

namespace UserSetting.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : StanWindow
{
    private readonly UserPreference _userPreference;

    public MainWindow()
    {
        InitializeComponent();

        _userPreference = new UserPreference();
        Loaded += MainWindowLoaded;
        Closing += MainWindowClosing;
    }

    private void MainWindowLoaded(object sender, RoutedEventArgs e)
    {
        _userPreference.Apply(this);
    }

    private void MainWindowClosing(object? sender, CancelEventArgs e)
    {
        UserPreference.Save(this);
    }

}