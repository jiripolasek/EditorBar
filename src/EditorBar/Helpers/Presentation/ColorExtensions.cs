// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Drawing;
using System.Runtime.CompilerServices;

namespace JPSoftworks.EditorBar.Helpers.Presentation;

/// <summary>
/// Helper for conversion between WinForms and WPF colors.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Converts a WPF color to a WinForms color.
    /// </summary>
    /// <param name="mediaColor">The WPF color.</param>
    /// <returns>The equivalent WinForms color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color ToDrawingColor(this System.Windows.Media.Color mediaColor)
    {
        // ReSharper disable ExceptionNotDocumentedOptional
        return Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);

        // ReSharper restore ExceptionNotDocumentedOptional
    }

    /// <summary>
    /// Converts a WinForms color to a WPF color.
    /// </summary>
    /// <param name="drawingColor">The WinForms color.</param>
    /// <returns>The equivalent WPF color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Color ToMediaColor(this Color drawingColor)
    {
        return System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
    }
}
