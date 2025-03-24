// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace JPSoftworks.EditorBar.Presentation;

internal sealed class ContrastForegroundBrushConverter : IMultiValueConverter
{
    // The contrast ratio threshold you’d like to enforce.
    // For example, a ratio < 4.5 means insufficient contrast for normal text.
    public double ContrastRatioThreshold { get; set; } = 2.5;

    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return null;
        }

        if (values[0] is not SolidColorBrush backgroundBrush ||
            values[1] is not SolidColorBrush foregroundBrush)
        {
            return values[1]; // fallback: just return the original foreground
        }

        // Calculate contrast ratio
        var backgroundLuminance = GetRelativeLuminance(backgroundBrush.Color);
        var foregroundLuminance = GetRelativeLuminance(foregroundBrush.Color);

        var ratio = (Math.Max(backgroundLuminance, foregroundLuminance) + 0.05) /
                    (Math.Min(backgroundLuminance, foregroundLuminance) + 0.05);

        // If the ratio is below the threshold, invert the foreground color
        if (ratio < this.ContrastRatioThreshold)
        {
            var inverted = InvertColor(foregroundBrush.Color);
            return new SolidColorBrush(inverted);
        }

        // Otherwise, keep the original foreground
        return foregroundBrush;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Calculates the relative luminance of a color using the W3C formula.
    /// Reference: https://www.w3.org/TR/WCAG20/#relativeluminancedef
    /// </summary>
    private static double GetRelativeLuminance(Color color)
    {
        // Convert to [0,1] range
        var r = color.R / 255.0;
        var g = color.G / 255.0;
        var b = color.B / 255.0;

        // Apply gamma expansion
        r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
        g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
        b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);

        // Calculate the luminance
        return (0.2126 * r) + (0.7152 * g) + (0.0722 * b);
    }

    /// <summary>
    /// Inverts a color by subtracting each channel from 255.
    /// </summary>
    private static Color InvertColor(Color color)
    {
        return Color.FromArgb(
            color.A,
            (byte)(255 - color.R),
            (byte)(255 - color.G),
            (byte)(255 - color.B));
    }
}