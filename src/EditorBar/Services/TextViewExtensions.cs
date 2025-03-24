// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Controls;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services;

internal static class TextViewExtensions
{
    internal static IStructureProvider? GetCurrentFileStructureProvider(this ITextView textView)
    {
        Requires.NotNull(textView, nameof(textView));

        textView.Properties!.TryGetProperty(typeof(IStructureProvider), out IStructureProvider? provider);
        return provider;
    }

    internal static void SetCurrentFileStructureProvider(this ITextView textView, IStructureProvider? provider)
    {
        Requires.NotNull(textView, nameof(textView));

        textView.Properties![typeof(IStructureProvider)] = provider!;
    }

    // get / set EditorBarControl
    internal static EditorBarControl? GetEditorBarControl(this ITextView textView)
    {
        Requires.NotNull(textView, nameof(textView));
        textView.Properties!.TryGetProperty(typeof(EditorBarControl), out EditorBarControl? control);
        return control;
    }

    internal static void SetEditorBarControl(this ITextView textView, EditorBarControl? control)
    {
        Requires.NotNull(textView, nameof(textView));
        textView.Properties![typeof(EditorBarControl)] = control!;
    }
}