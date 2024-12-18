// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.ComponentModel.Composition;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar.MefComponents;

/// <summary>
/// Base for a factory for creating a margin that will hold the Editor bar.
/// </summary>
/// <seealso cref="Microsoft.VisualStudio.Text.Editor.IWpfTextViewMarginProvider" />
abstract class BaseEditorBarFactory(BarPosition targetBarPosition) : IWpfTextViewMarginProvider
{
    [Import]
    internal JoinableTaskContext JoinableTaskContext = null!;

    [Import]
    internal SVsServiceProvider ServiceProvider = null!;

    private IWpfTextView? _textView;

    /// <inheritdoc />
    public IWpfTextViewMargin? CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
    {
        this._textView = wpfTextViewHost.TextView;
        if (this._textView == null)
            return null;

        return new EditorBarMargin(
            this._textView,
            this.JoinableTaskContext.Factory,
            this.ServiceProvider,
            targetBarPosition);
    }
}