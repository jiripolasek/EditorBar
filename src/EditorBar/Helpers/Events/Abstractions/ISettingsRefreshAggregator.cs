// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Helpers.Events.Abstractions;

/// <summary>
/// An interface for aggregating settings refresh events. It includes an event that triggers when editor bar settings
/// need to be refreshed.
/// </summary>
public interface ISettingsRefreshAggregator : IDisposable
{
    /// <summary>
    /// Raised whenever editor bar settings should be refreshed
    /// (e.g., on an options save or environment color change).
    /// </summary>
    event EventHandler<SettingsRefreshEventArgs>? SettingsRefreshRequested;
}