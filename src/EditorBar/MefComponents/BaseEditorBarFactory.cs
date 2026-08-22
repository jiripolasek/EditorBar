// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
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
internal abstract class BaseEditorBarFactory(BarPosition targetBarPosition, string marginName)
    : IWpfTextViewMarginProvider
{
    [Import]
    private JoinableTaskContext JoinableTaskContext { get; set; } = null!;

    [Import]
    private SVsServiceProvider ServiceProvider { get; set; } = null!;

    [Import]
    private IStructureProviderService StructureProviderService { get; set; } = null!;

    /// <inheritdoc />
    public IWpfTextViewMargin? CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
    {
        if (wpfTextViewHost.TextView == null)
        {
            return null;
        }

        var textView = wpfTextViewHost.TextView;

        return new EditorBarMargin(
            textView,
            this.JoinableTaskContext.Factory,
            this.ServiceProvider,
            this.StructureProviderService,
            targetBarPosition,
            marginName);
    }
}
