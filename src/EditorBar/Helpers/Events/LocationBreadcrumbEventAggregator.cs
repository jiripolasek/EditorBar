// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Helpers.Events.Abstractions;
using JPSoftworks.EditorBar.Services;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Helpers.Events;

/// <summary>
/// Aggregates multiple event sources that affect location breadcrumbs
/// and raises a single "RefreshRequested" event.
/// </summary>
internal sealed class LocationBreadcrumbEventAggregator : IDisposable
{
    public event EventHandler? RefreshRequested;

    private readonly SolutionProjectChangeEventAggregator _solutionProjectChangeEventAggregator;
    private readonly ITextDocument _textDocument;
    private readonly IWorkspaceMonitor _workspaceMonitor;

    public LocationBreadcrumbEventAggregator(
        ITextDocument textDocument,
        IWorkspaceMonitor workspaceMonitor,
        SolutionProjectChangeEventAggregator solutionProjectChangeEventAggregator)
    {
        Requires.NotNull(textDocument, nameof(textDocument));
        Requires.NotNull(workspaceMonitor, nameof(workspaceMonitor));
        Requires.NotNull(solutionProjectChangeEventAggregator, nameof(solutionProjectChangeEventAggregator));

        this._workspaceMonitor = workspaceMonitor;
        this._solutionProjectChangeEventAggregator = solutionProjectChangeEventAggregator;

        // Hook workspace monitor events that could affect location:
        this._workspaceMonitor.DocumentActiveContextChanged += this.OnDocumentActiveContextChanged;
        this._workspaceMonitor.WorkspaceChanged += this.OnWorkspaceChanged;

        // Hook solution/project changes:
        this._solutionProjectChangeEventAggregator.Changed += this.OnSolutionProjectChanged;

        // Hook text document file actions
        this._textDocument = textDocument;
        this._textDocument.FileActionOccurred += this.OnTextDocumentFileActionOccurred;
    }

    public void Dispose()
    {
        this._workspaceMonitor.DocumentActiveContextChanged -= this.OnDocumentActiveContextChanged;
        this._workspaceMonitor.WorkspaceChanged -= this.OnWorkspaceChanged;

        this._solutionProjectChangeEventAggregator.Changed -= this.OnSolutionProjectChanged;

        this._textDocument.FileActionOccurred -= this.OnTextDocumentFileActionOccurred;
    }

    private void OnDocumentActiveContextChanged(object sender, DocumentActiveContextChangedEventArgs e)
    {
        this.RaiseRefreshRequested();
    }

    private void OnWorkspaceChanged(object sender, Workspace? e)
    {
        this.RaiseRefreshRequested();
    }

    private void OnSolutionProjectChanged(object sender, SolutionProjectChangedEventArgs e)
    {
        this.RaiseRefreshRequested();
    }

    private void OnTextDocumentFileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
    {
        this.RaiseRefreshRequested();
    }

    private void RaiseRefreshRequested()
    {
        this.RefreshRequested?.Invoke(this, EventArgs.Empty!);
    }
}