// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace JPSoftworks.EditorBar.Presentation;

internal class IsFirstItemConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is FrameworkElement { DataContext: not null } currentItem &&
            values[1] is ItemsControl itemsControl)
        {
            // Get the index of the current item in the ItemsControl
            var index = itemsControl.Items.IndexOf(currentItem.DataContext);
            return index == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Visible;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}