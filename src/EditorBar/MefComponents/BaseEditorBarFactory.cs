// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.ComponentModel.Composition;
using JPSoftworks.EditorBar.Options;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar.MefComponents;

/// <summary>
/// Base for a factory for creating a margin that will hold the Editor bar.
/// </summary>
/// <seealso cref="Microsoft.VisualStudio.Text.Editor.IWpfTextViewMarginProvider" />
internal abstract class BaseEditorBarFactory(BarPosition targetBarPosition) : IWpfTextViewMarginProvider
{
    private IWpfTextView? _textView;

    [Import] internal JoinableTaskContext JoinableTaskContext = null!;

    [Import] internal SVsServiceProvider ServiceProvider = null!;

    [Import] internal IStructureProviderService StructureProviderService = null!;

    /// <inheritdoc />
    public IWpfTextViewMargin? CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
    {
        if (wpfTextViewHost.TextView == null)
        {
            return null;
        }

        this._textView = wpfTextViewHost.TextView;

        return new EditorBarMargin(
            this._textView,
            this.JoinableTaskContext.Factory,
            this.ServiceProvider,
            this.StructureProviderService,
            targetBarPosition);
    }
}