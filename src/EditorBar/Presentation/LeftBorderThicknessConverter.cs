// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace JPSoftworks.EditorBar.Presentation;

internal class LeftBorderThicknessConverter : IMultiValueConverter
{
    public object Convert(object?[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 2)
        {
            return new Thickness(0);
        }

        // If we don't have a "previous item" (e.g. first item in list),
        // WPF passes DependencyProperty.UnsetValue or null for the second param.
        return values[1] == DependencyProperty.UnsetValue || values[1] is not Brush
            ? new Thickness(0)
            : new Thickness(2);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}