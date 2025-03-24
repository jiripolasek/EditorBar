// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Helpers.Events.Abstractions;

/// <summary>
/// Provides the reason for a settings refresh request.
/// </summary>
public sealed class SettingsRefreshEventArgs : EventArgs
{
    /// <summary>
    /// Gets the reason for the settings refresh request.
    /// </summary>
    public SettingsRefreshReason Reason { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsRefreshEventArgs" /> class.
    /// </summary>
    /// <param name="reason">The reason for the settings refresh request.</param>
    public SettingsRefreshEventArgs(SettingsRefreshReason reason)
    {
        this.Reason = reason;
    }
}