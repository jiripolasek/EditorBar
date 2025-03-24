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
using JPSoftworks.EditorBar.Helpers.Presentation;
using Microsoft.VisualStudio.PlatformUI;

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
        var separatorBrush = (Brush)Application.Current!.FindResource(SearchControlColors.PopupBorderBrushKey!);

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
}

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