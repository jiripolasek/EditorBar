// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// Opens a terminal rooted at the current document location.
/// </summary>
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_OpenTerminalCommand)]
internal sealed class OpenTerminalCommand : BaseFileActionMenuContextCommand<OpenTerminalCommand>
{
    protected override Task ExecuteCoreAsync(string filePath)
    {
        Launcher.OpenTerminal(filePath);
        return Task.CompletedTask;
    }
}
