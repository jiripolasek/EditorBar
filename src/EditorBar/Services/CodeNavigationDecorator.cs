// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel.Composition;
using Microsoft;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace JPSoftworks.EditorBar.Services;

/// <summary>
/// Registers the <see cref="CodeNavigationService" /> as a property of the <see cref="IWpfTextView" />.
/// </summary>
/// <seealso cref="Microsoft.VisualStudio.Text.Editor.IWpfTextViewCreationListener" />
[Export(typeof(IWpfTextViewCreationListener))]
[ContentType("Any")]
[TextViewRole(PredefinedTextViewRoles.Structured)]
internal class CodeNavigationDecorator : IWpfTextViewCreationListener
{
    /// <summary>
    /// Called when a text view having matching roles is created over a text data model having a matching content type.
    /// </summary>
    /// <param name="textView">The newly created text view.</param>
    public void TextViewCreated(IWpfTextView textView)
    {
        Requires.NotNull(textView, nameof(textView));

        var codeNavigationService = new CodeNavigationService(textView);
        textView.Properties![typeof(ICodeNavigationService)] = codeNavigationService;
        textView.Properties![typeof(ITextNavigationService)] = codeNavigationService;
    }
}