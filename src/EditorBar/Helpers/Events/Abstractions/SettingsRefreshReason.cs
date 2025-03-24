// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Helpers.Events.Abstractions;

/// <summary>
/// Enumerates the various reasons a settings refresh can be triggered.
/// </summary>
public enum SettingsRefreshReason
{
    /// <summary>
    /// Triggered when options are saved.
    /// </summary>
    OptionsSaved,

    /// <summary>
    /// Triggered when the environment color is changed.
    /// </summary>
    EnvironmentColorChanged
}