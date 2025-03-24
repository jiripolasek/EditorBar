// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace JPSoftworks.EditorBar.Presentation;

internal class NullToCollapsedConverter : IValueConverter
{
    public NullToCollapsedConverter()
    {
        Console.WriteLine();
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || value == DependencyProperty.UnsetValue)
        {
            return Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}