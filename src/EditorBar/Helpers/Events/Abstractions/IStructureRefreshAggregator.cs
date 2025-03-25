// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Helpers.Events.Abstractions;

/// <summary>
/// An interface for aggregating file structure refresh events.
/// </summary>
internal interface IStructureRefreshAggregator : IDisposable
{
    /// <summary>
    /// Raised whenever a file structure refresh is requested
    /// due to content type or workspace changes.
    /// </summary>
    event EventHandler? RefreshRequested;
}