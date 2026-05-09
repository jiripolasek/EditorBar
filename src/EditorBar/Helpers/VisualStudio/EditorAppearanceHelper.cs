// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Windows.Media;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;
using DrawingColor = System.Drawing.Color;

namespace JPSoftworks.EditorBar.Helpers.VisualStudio;

/// <summary>
/// Resolves the effective editor appearance mode used for theme-specific Editor Bar colors.
/// </summary>
public static class EditorAppearanceHelper
{
    public static EditorColorMode GetCurrentMode(IWpfTextView? textView = null)
    {
        var editorBackground = TryGetEditorBackgroundColor(textView);
        if (editorBackground is { A: > 0 } color)
        {
            return GetMode(color);
        }

        return GetMode(ToMediaColor(VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey)));
    }

    private static Color? TryGetEditorBackgroundColor(IWpfTextView? textView)
    {
        var visualElement = textView?.VisualElement;
        if (visualElement == null)
        {
            return null;
        }

        var dispatcher = visualElement.Dispatcher;
        if (dispatcher == null)
        {
            return null;
        }

        if (!dispatcher.CheckAccess())
        {
            return null;
        }

        return (textView!.Background as SolidColorBrush)?.Color;
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
