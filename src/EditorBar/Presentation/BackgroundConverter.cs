// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.Windows.Media;
using JPSoftworks.EditorBar.Helpers.Presentation;
using Microsoft.VisualStudio.PlatformUI;

namespace JPSoftworks.EditorBar.Presentation;

/// <summary>
/// A converter that provides a fallback color for a <see cref="SolidColorBrush" />.
/// </summary>
public class SolidColorFallbackConverter : MultiValueConverter<Brush?, Brush?, Brush?>
{
    /// <inheritdoc />
    protected override Brush? Convert(Brush? value1, Brush? value2, object parameter, CultureInfo culture)
    {
        if (!BrushHelper.AreBrushesEqual(value1, Brushes.Transparent))
        {
            return value1;
        }

        if (!BrushHelper.AreBrushesEqual(value2, Brushes.Transparent))
        {
            return value2;
        }

        return Brushes.Fuchsia;
    }
}