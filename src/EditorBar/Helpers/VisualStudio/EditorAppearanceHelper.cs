// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using DrawingColor = System.Drawing.Color;
using System.Windows.Media;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Helpers.VisualStudio;

/// <summary>
/// Resolves the effective editor appearance mode used for theme-specific Editor Bar colors.
/// </summary>
public static class EditorAppearanceHelper
{
    public static EditorColorMode GetCurrentMode(IWpfTextView? textView = null)
    {
        var brush = textView?.Background as SolidColorBrush;
        if (brush is { Color.A: > 0 })
        {
            return GetMode(brush.Color);
        }

        return GetMode(ToMediaColor(VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey)));
    }

    private static EditorColorMode GetMode(Color color)
    {
        var luminance = ((0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B)) / 255d;
        return luminance < 0.5 ? EditorColorMode.Dark : EditorColorMode.Light;
    }

    private static Color ToMediaColor(DrawingColor color)
    {
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }
}
