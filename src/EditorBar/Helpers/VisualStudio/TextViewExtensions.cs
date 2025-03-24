// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Services;
using Microsoft;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Provides extension methods for <see cref="ITextView" />.
/// </summary>
internal static class TextViewExtensions
{
    /// <summary>
    /// Gets the <see cref="ITextDocument" /> from the document buffer of the specified <see cref="ITextView" />.
    /// </summary>
    /// <param name="textView">The text view.</param>
    /// <returns>The <see cref="ITextDocument" /> if found; otherwise, <c>null</c>.</returns>
    public static ITextDocument? GetTextDocumentFromDocumentBuffer(this ITextView textView)
    {
        Requires.NotNull(textView, nameof(textView));

        return textView.TextDataModel?.DocumentBuffer?.Properties!.TryGetProperty<ITextDocument>(typeof(ITextDocument),
            out var document) == true
            ? document
            : null;
    }

    /// <summary>
    /// Gets the <see cref="ICodeNavigationService" /> for the specified <see cref="ITextView" />.
    /// </summary>
    /// <param name="textView">The text view.</param>
    /// <returns>The <see cref="ICodeNavigationService" />.</returns>
    public static ICodeNavigationService GetCodeNavigationService(this ITextView textView)
    {
        Requires.NotNull(textView, nameof(textView));

        return textView.Properties!.GetOrCreateSingletonProperty(typeof(ICodeNavigationService),
            () => new CodeNavigationService(textView))!;
    }

    /// <summary>
    /// Gets the <see cref="ITextNavigationService" /> for the specified <see cref="ITextView" />.
    /// </summary>
    /// <param name="textView">The text view.</param>
    /// <returns>The <see cref="ITextNavigationService" />.</returns>
    public static ITextNavigationService GetTextNavigationService(this ITextView textView)
    {
        Requires.NotNull(textView, nameof(textView));

        return textView.Properties!.GetOrCreateSingletonProperty(typeof(ITextNavigationService),
            () => new CodeNavigationService(textView))!;
    }
}