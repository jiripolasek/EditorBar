// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Dialogs;

internal sealed class WhatsNewDisplayContext
{
    internal WhatsNewDisplayContext(string? previouslySeenVersion, bool isFirstRun, bool isAutomatic)
    {
        this.PreviouslySeenVersion = System.Version.TryParse(previouslySeenVersion, out var version) ? version : null;
        this.CurrentVersion = System.Version.Parse(Vsix.Version);
        this.IsFirstRun = isFirstRun;
        this.IsAutomatic = isAutomatic;
    }

    internal Version? PreviouslySeenVersion { get; }

    internal Version CurrentVersion { get; }

    internal bool IsFirstRun { get; }

    internal bool IsAutomatic { get; }
}
