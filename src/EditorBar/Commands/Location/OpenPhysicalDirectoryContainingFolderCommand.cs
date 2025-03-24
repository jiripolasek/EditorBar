// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.ViewModels;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarPhysicalDirectoryMenu_OpenContainingFolderCommand)]
internal sealed class OpenPhysicalDirectoryContainingFolderCommand :
    BasePhysicalLocationContextMenuCommand<OpenPhysicalDirectoryContainingFolderCommand>
{
    protected override Task ExecuteCoreAsync(PhysicalDirectoryModel physicalDirectory)
    {
        Launcher.OpenFolder(physicalDirectory.FullPath);
        return Task.CompletedTask;
    }
}