// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace JPSoftworks.EditorBar.Presentation;

internal class BreadcrumbSeparatorColorConverter : IMultiValueConverter
{
    public object Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 3)
        {
            return Brushes.Transparent;
        }

        // parent background
        // current background
        // next element background (if available; might be null or unset if there's no next element)
        // stroke color
        var parentBg = values[0] as Brush ?? Brushes.Transparent;
        var currentBg = values[1] as Brush ?? Brushes.Transparent;
        var nextBg = values[2] as Brush ?? Brushes.Transparent;
        var separatorBrush = CreateSeparatorBrush(parentBg);

        // we use parent background as a separator color if possible: current and parent background have both specific colors and they are both different from the parent bg
        // if parent and current background are the same, and they match the color of background or are transaprent then we have to use separatorBrush

        // normalize next and current colors to transparent, if they are the same as parent color
        if (AreBrushesEqual(nextBg, parentBg))
        {
            nextBg = Brushes.Transparent;
        }

        if (AreBrushesEqual(currentBg, parentBg))
        {
            currentBg = Brushes.Transparent;
        }

        if (AreBrushesEqual(nextBg, currentBg) &&
            (AreBrushesEqual(currentBg, parentBg) || AreBrushesEqual(currentBg, Brushes.Transparent)))
        {
            return separatorBrush;
        }

        return parentBg;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static bool AreBrushesEqual(Brush? brush1, Brush? brush2)
    {
        // Check if both brushes are SolidColorBrush
        if (brush1 is SolidColorBrush solidBrush1 && brush2 is SolidColorBrush solidBrush2)
        {
            // Compare the Colors
            return solidBrush1.Color == solidBrush2.Color;
        }

        // If brushes are not SolidColorBrush, return false as they can't be directly compared
        return false;
    }

    private static Brush CreateSeparatorBrush(Brush backgroundBrush)
    {
        if (backgroundBrush is not SolidColorBrush solidBackground)
        {
            return Brushes.Gray;
        }

        var luminance = ((0.2126 * solidBackground.Color.R) +
                         (0.7152 * solidBackground.Color.G) +
                         (0.0722 * solidBackground.Color.B)) / 255d;

        var color = luminance < 0.5
            ? Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF)
            : Color.FromArgb(0x66, 0x00, 0x00, 0x00);

        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
}
