// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace JPSoftworks.EditorBar.Presentation;

/// <summary>
/// Converts a Color object to a SolidColorBrush object.
/// </summary>
public class ColorToBrushConverter : IValueConverter
{
    /// <summary>
    /// Converts a Color object to a SolidColorBrush object.
    /// </summary>
    /// <param name="value">The Color object to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A SolidColorBrush object representing the converted Color.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is Color color ? new SolidColorBrush(color) : null;
    }

    /// <summary>
    /// Converts a SolidColorBrush object to a Color object.
    /// </summary>
    /// <param name="value">The SolidColorBrush object to convert.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A Color object representing the converted SolidColorBrush.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is SolidColorBrush brush ? brush.Color : null;
    }
}
