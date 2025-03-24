// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows.Media;

namespace JPSoftworks.EditorBar.Helpers.Presentation;

/// <summary>
/// Provides helper methods for working with brushes in the presentation layer.
/// </summary>
internal static class BrushHelper
{
    /// <summary>
    /// Determines whether two <see cref="Brush" /> instances are equal.
    /// </summary>
    /// <param name="brush1">The first brush to compare.</param>
    /// <param name="brush2">The second brush to compare.</param>
    /// <returns>
    /// True if both brushes are <see cref="SolidColorBrush" /> instances and their colors are equal; otherwise, false.
    /// </returns>
    internal static bool AreBrushesEqual(Brush? brush1, Brush? brush2)
    {
        return brush1 is SolidColorBrush solidBrush1
               && brush2 is SolidColorBrush solidBrush2
               && solidBrush1.Color == solidBrush2.Color;
    }
}