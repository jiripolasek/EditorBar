// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Helpers.Events.Abstractions;
using JPSoftworks.EditorBar.Services;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Helpers.Events;

internal sealed class StructureRefreshAggregator : IStructureRefreshAggregator
{
    public event EventHandler? RefreshRequested;
    private readonly IWpfTextView _textView;
    private readonly IWorkspaceMonitor _workspaceMonitor;

    public StructureRefreshAggregator(
        IWpfTextView textView,
        IWorkspaceMonitor workspaceMonitor)
    {
        this._textView = textView;
        this._workspaceMonitor = workspaceMonitor;

        this._textView.TextBuffer.ContentTypeChanged += this.OnContentTypeChanged;
        this._workspaceMonitor.WorkspaceChanged += this.OnWorkspaceChanged;
    }

    public void Dispose()
    {
        this._textView.TextBuffer.ContentTypeChanged -= this.OnContentTypeChanged;
        this._workspaceMonitor.WorkspaceChanged -= this.OnWorkspaceChanged;
    }

    private void OnContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
    {
        this.RaiseRefreshRequested();
    }

    private void OnWorkspaceChanged(object sender, Workspace? e)
    {
        this.RaiseRefreshRequested();
    }

    private void RaiseRefreshRequested()
    {
        this.RefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}