// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services;

/// <summary>
/// Monitors workspace changes and manages subscriptions to workspace events for a specific text view.
/// </summary>
internal class WorkspaceMonitor : IWorkspaceMonitor
{
    public event EventHandler<DocumentActiveContextChangedEventArgs>? DocumentActiveContextChanged;
    public event EventHandler<Workspace?>? WorkspaceChanged;

    private readonly object _workspaceLock = new();
    private readonly WorkspaceRegistration _workspaceRegistration;

    public Workspace? CurrentWorkspace { get; private set; }

    public WorkspaceMonitor(IWpfTextView textView)
    {
        Requires.NotNull(textView, nameof(textView));

        this._workspaceRegistration = Workspace.GetWorkspaceRegistration(textView.TextBuffer!.AsTextContainer());
        this._workspaceRegistration.WorkspaceChanged += this.OnWorkspaceRegistrationChanged;

        this.UpdateCurrentWorkspace(this._workspaceRegistration.Workspace);
    }

    public void Dispose()
    {
        this._workspaceRegistration.WorkspaceChanged -= this.OnWorkspaceRegistrationChanged;

        if (this.CurrentWorkspace != null)
        {
            this.CurrentWorkspace.DocumentActiveContextChanged -= this.OnDocumentActiveContextChanged;
        }

        this.WorkspaceChanged = null;
        this.DocumentActiveContextChanged = null;
    }

    private void OnWorkspaceRegistrationChanged(object sender, EventArgs e)
    {
        this.UpdateCurrentWorkspace(this._workspaceRegistration.Workspace);
    }

    private void UpdateCurrentWorkspace(Workspace? newWorkspace)
    {
        if (this.CurrentWorkspace == newWorkspace)
        {
            return;
        }

        lock (this._workspaceLock)
        {
            if (this.CurrentWorkspace == newWorkspace)
            {
                return;
            }

            if (this.CurrentWorkspace != null)
            {
                this.CurrentWorkspace.DocumentActiveContextChanged -= this.OnDocumentActiveContextChanged;
            }

            this.CurrentWorkspace = newWorkspace;

            if (this.CurrentWorkspace != null)
            {
                this.CurrentWorkspace.DocumentActiveContextChanged += this.OnDocumentActiveContextChanged;
            }

            this.WorkspaceChanged?.Invoke(this, newWorkspace);
        }
    }

    private void OnDocumentActiveContextChanged(object sender, DocumentActiveContextChangedEventArgs e)
    {
        this.DocumentActiveContextChanged?.Invoke(sender, e);
    }
}