using System.Windows;

namespace UserSetting.Controls.Proxies;

/// <summary>
/// A freezable class that provides a binding proxy to any object
/// </summary>
/// <example>Displaying PosID in OrderListingItemView</example
/// <seealso cref="Freezable" />
/// <seealso cref="BindingProxy" />
/// <reference>
/// "https://thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/"
/// </reference>
public class BindingProxy : Freezable
{

    public object Data
    {
        get { return GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new UIPropertyMetadata(default));

    #region Overrides of Freezable

    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }

    #endregion Overrides of Freezable
}

