// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using JPSoftworks.EditorBar.Helpers.Presentation;

namespace JPSoftworks.EditorBar.Presentation;

public class SeparatorStrokeConverter : IMultiValueConverter
{
    public object? Convert(object?[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 4)
        {
            return Brushes.Transparent;
        }

        var currentBg = values[0] as Brush;
        var previousBg = values[1] as Brush;

        var transparentBg = values[2] as Brush;
        var separatorBrush = values[3] as Brush;

        // If we don't have a "previous item" (e.g. first item in list),
        // WPF passes DependencyProperty.UnsetValue or null for the second param.
        if (values[1] == DependencyProperty.UnsetValue || previousBg == null)
        {
            return Brushes.Transparent;
        }

        // if previous and following brushes are the same, and they match the color of background then we should use separatorBrush
        if (BrushHelper.AreBrushesEqual(currentBg, previousBg) &&
            (BrushHelper.AreBrushesEqual(currentBg, transparentBg) ||
             BrushHelper.AreBrushesEqual(currentBg, Brushes.Transparent)))
        {
            return separatorBrush;
        }

        return transparentBg;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
