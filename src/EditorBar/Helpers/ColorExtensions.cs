﻿// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Helper for conversion between WinForms and WPF colors.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Convert Media Color (WPF) to Drawing Color (WinForms).
    /// </summary>
    /// <param name="mediaColor">WPF color.</param>
    /// <returns>WinForms Color equivalent to <paramref name="mediaColor"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color mediaColor)
    {
        // ReSharper disable ExceptionNotDocumentedOptional
        return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
        // ReSharper restore ExceptionNotDocumentedOptional
    }

    /// <summary>
    /// Convert Drawing Color (WinForms) to Media Color (WPF).
    /// </summary>
    /// <param name="drawingColor"></param>
    /// <returns>WPF Media Color equivalent to <paramref name="drawingColor"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color drawingColor)
    {
        return System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
    }
}