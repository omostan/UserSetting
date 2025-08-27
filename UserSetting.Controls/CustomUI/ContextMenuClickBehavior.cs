﻿using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace UserSetting.Controls.CustomUI;

/// <summary>
/// Reef1: https://stackoverflow.com/a/29123964/5352166
/// Ref2: https://www.codeproject.com/Tips/1136681/WPF-Drop-Down-Menu-Button
/// </summary>
public class ContextMenuClickBehavior
{
    public static bool GetIsLeftClickEnabled(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsLeftClickEnabledProperty);
    }

    public static void SetIsLeftClickEnabled(DependencyObject obj, bool value)
    {
        obj.SetValue(IsLeftClickEnabledProperty, value);
    }

    public static readonly DependencyProperty IsLeftClickEnabledProperty = DependencyProperty.RegisterAttached(
        "IsLeftClickEnabled",
        typeof(bool),
        typeof(ContextMenuClickBehavior),
        new UIPropertyMetadata(false, OnIsLeftClickEnabledChanged));

    private static void OnIsLeftClickEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        var uiElement = (UIElement?)sender;

        var isEnabled = (bool)e.NewValue && (bool)e.NewValue;
        if (uiElement == null)
        {
            return;
        }

        if (isEnabled)
        {
            if (uiElement is ButtonBase @base)
                @base.Click += OnMouseLeftButtonUp;
            else
                uiElement.MouseLeftButtonUp += OnMouseLeftButtonUp;
        }
        else
        {
            if (uiElement is ButtonBase @base)
                @base.Click -= OnMouseLeftButtonUp;
            else
                uiElement.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        }
    }

    private static void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
    {
        //Debug.Print("OnMouseLeftButtonUp");
        var fe = (FrameworkElement?)sender;

        if (fe == null)
        {
            return;
        }

        // if we use binding in our context menu, then it's DataContext won't be set when we show the menu on left click
        // (it seems setting DataContext for ContextMenu is hardcoded in WPF when user right-clicks on a control, although I'm not sure)
        // so we have to set up ContextMenu.DataContext manually here
        if (fe.ContextMenu?.DataContext == null)
        {
            fe.ContextMenu?.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = fe.DataContext });
        }

        fe.ContextMenu!.IsOpen = true;
    }
}
