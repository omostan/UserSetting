using System.Windows;

namespace UserSetting.WPF.CustomUI;

internal class AppTheme
{
    public static void ChangeTheme(Uri themeUri)
    {
        ResourceDictionary appTheme = new() { Source = themeUri };
        Application.Current.Resources.Clear();
        Application.Current.Resources.MergedDictionaries.Add(appTheme);
    }
}
