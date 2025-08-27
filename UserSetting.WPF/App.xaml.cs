using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using TraceTool;
using UserSetting.WPF.CustomUI;
using UserSetting.WPF.Properties;
using UserSetting.WPF.ViewModels;

namespace UserSetting.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    #region Fields

    private readonly IHost _host;

    private const string BaseUri = "pack://application:,,,/UserSetting.Controls;component/Themes";

    #endregion Fields

    #region App

    public App()
    {
        _host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    // In ConfigureServices
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton(provider => new Lazy<MainViewModel>(provider.GetRequiredService<MainViewModel>));
                    services.AddSingleton(provider => new MainWindow
                    {
                        DataContext = provider.GetRequiredService<Lazy<MainViewModel>>().Value
                    });
                })
                .Build();
    }

    #endregion App

    #region OnStartup

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host.Start();

        Task.Run( async () => await ApplyThemeAsync());

        var serviceProvider = _host.Services;

        MainWindow = serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    #endregion OnStartup

    #region OnExit

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            // Stop and dispose the host
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }
        catch (Exception ex)
        {
            TTrace.Error.Send("Error during application exit:", ex.Message);
        }
    }

    #endregion OnExit

    #region ApplyTheme

    /// <summary>
    /// Applies the user's selected theme to the application.
    /// </summary>
    private static async Task ApplyThemeAsync()
    {
        try
        {
            var theme = Settings.Default.Theme;
            await Task.Run(() => AppTheme.ChangeTheme(new Uri($"{BaseUri}/{theme}Theme.xaml")));
        }
        catch (Exception ex)
        {
            TTrace.Error.Send(ex.Message);
        }
    }
    
    /*private static void ApplyTheme()
    {
        try
        {
            var theme = Settings.Default.Theme;
            AppTheme.ChangeTheme(new Uri($"{BaseUri}/{theme}Theme.xaml"));
        }
        catch (Exception ex)
        {
            TTrace.Error.Send(ex.Message);
        }
    }*/

    #endregion ApplyTheme
}