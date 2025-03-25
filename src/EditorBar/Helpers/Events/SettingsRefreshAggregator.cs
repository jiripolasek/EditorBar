// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers.Events.Abstractions;
using JPSoftworks.EditorBar.Options;

namespace JPSoftworks.EditorBar.Helpers.Events;

/// <summary>
/// Aggregates settings refresh events from various sources.
/// </summary>
public sealed class SettingsRefreshAggregator : ISettingsRefreshAggregator
{
    /// <inheritdoc />
    public event EventHandler<SettingsRefreshEventArgs>? SettingsRefreshRequested;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsRefreshAggregator" /> class.
    /// </summary>
    public SettingsRefreshAggregator()
    {
        GeneralOptionsModel.Saved += this.OnGeneralOptionsSaved;
        VS.Events.ShellEvents.EnvironmentColorChanged += this.OnEnvironmentColorChanged;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GeneralOptionsModel.Saved -= this.OnGeneralOptionsSaved;
        VS.Events.ShellEvents.EnvironmentColorChanged -= this.OnEnvironmentColorChanged;
    }

    private void OnGeneralOptionsSaved(GeneralOptionsModel model)
    {
        this.RaiseSettingsRefresh(SettingsRefreshReason.OptionsSaved);
    }

    private void OnEnvironmentColorChanged()
    {
        this.RaiseSettingsRefresh(SettingsRefreshReason.EnvironmentColorChanged);
    }

    private void RaiseSettingsRefresh(SettingsRefreshReason reason)
    {
        this.SettingsRefreshRequested?.Invoke(
            this,
            new SettingsRefreshEventArgs(reason));
    }
}