// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.ComponentModel.Composition;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar;

/// <summary>
/// Base for a factory for creating a margin that will hold the Editor bar.
/// </summary>
/// <seealso cref="Microsoft.VisualStudio.Text.Editor.IWpfTextViewMarginProvider" />
abstract class BaseEditorBarFactory(BarPosition targetBarPosition) : IWpfTextViewMarginProvider
{
    [Import]
    internal JoinableTaskContext JoinableTaskContext = null!;

    private IWpfTextView? _textView;
    private BarPosition TargetBarPosition { get; } = targetBarPosition;

    /// <inheritdoc />
    public IWpfTextViewMargin? CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
    {
        this._textView = wpfTextViewHost.TextView;
        return this._textView != null ? new EditorBarMargin(this._textView, JoinableTaskContext.Factory, this.TargetBarPosition) : null;
    }
}