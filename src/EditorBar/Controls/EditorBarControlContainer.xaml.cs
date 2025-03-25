// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Windows;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar.Controls;

/// <summary>
/// This container is ugly wrapper around <see cref="EditorBarControl" /> to allow for dynamic loading and unloading of the
/// control. At least until I refactore it.
/// </summary>
/// <remarks>
/// It seems that <see cref="IWpfTextViewMargin" /> has to always return some control, so I have to create this wrapper to
/// allow for dynamic loading and unloading of the control.
/// </remarks>
internal partial class EditorBarControlContainer : IDisposable
{
    private readonly JoinableTaskFactory _joinableTaskFactory;
    private readonly IStructureProviderService _structureProviderService;
    private readonly IWpfTextView _textView;

    public EditorBarControlContainer(
        IWpfTextView textView,
        JoinableTaskFactory joinableTaskFactory,
        IStructureProviderService structureProviderService,
        bool visible)
    {
        this._textView = textView;
        this._joinableTaskFactory = joinableTaskFactory;
        this._structureProviderService = structureProviderService;
        this.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        this.InitializeComponent();
        this.RebuildUI(visible);
    }

    public void Dispose()
    {
        this.DisposeCurrentContent();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == VisibilityProperty && e.NewValue is Visibility newVisibility)
        {
            this.RebuildUI(newVisibility == Visibility.Visible);
        }
    }

    private void RebuildUI(bool visible)
    {
        if (visible && this.Content == null)
        {
            var textDocument = this._textView.GetTextDocumentFromDocumentBuffer();
            if (textDocument != null)
            {
                this.Content = new EditorBarControl(
                    this._textView,
                    textDocument,
                    this._joinableTaskFactory,
                    this._structureProviderService);
                return;
            }
        }

        this.DisposeCurrentContent();
        this.Content = null!;
    }

    private void DisposeCurrentContent()
    {
        if (this.Content is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}