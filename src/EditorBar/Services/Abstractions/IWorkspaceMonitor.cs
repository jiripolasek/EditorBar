// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.CodeAnalysis;

namespace JPSoftworks.EditorBar.Services;

/// <summary>
/// Monitors workspace changes and manages subscriptions to workspace events. Notifies subscribers when the current
/// workspace changes.
/// </summary>
internal interface IWorkspaceMonitor : IDisposable
{
    /// <summary>
    /// Raised when the active context of the document changes.
    /// </summary>
    event EventHandler<DocumentActiveContextChangedEventArgs>? DocumentActiveContextChanged;

    /// <summary>
    /// Raised when the current workspace associated with monitored object changes.
    /// </summary>
    event EventHandler<Workspace?>? WorkspaceChanged;

    /// <summary>
    /// Gets the current workspace associated with the context. Returns <see langword="null" /> if no workspace is set.
    /// </summary>
    Workspace? CurrentWorkspace { get; }
}