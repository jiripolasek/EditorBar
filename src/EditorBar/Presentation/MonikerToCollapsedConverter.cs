// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace JPSoftworks.EditorBar.Presentation;

internal class MonikerToCollapsedConverter : IValueConverter
{
    private static readonly ImageMoniker Empty = default;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || value == DependencyProperty.UnsetValue ||
            (value is ImageMoniker m && (Equals(m, KnownMonikers.None) || Equals(m, Empty))))
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