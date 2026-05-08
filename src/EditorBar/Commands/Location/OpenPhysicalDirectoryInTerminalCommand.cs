// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.ViewModels;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// Opens a terminal rooted at the selected physical directory.
/// </summary>
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarPhysicalDirectoryMenu_OpenTerminalCommand)]
internal sealed class OpenPhysicalDirectoryInTerminalCommand
    : BasePhysicalLocationContextMenuCommand<OpenPhysicalDirectoryInTerminalCommand>
{
    protected override Task ExecuteCoreAsync(PhysicalDirectoryModel physicalDirectory)
    {
        Launcher.OpenTerminal(physicalDirectory.FullPath);
        return Task.CompletedTask;
    }
}
