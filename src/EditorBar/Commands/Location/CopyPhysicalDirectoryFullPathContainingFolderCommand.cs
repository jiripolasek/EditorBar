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

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarPhysicalDirectoryMenu_CopyFullPathCommand)]
internal sealed class CopyPhysicalDirectoryFullPathContainingFolderCommand :
    BasePhysicalLocationContextMenuCommand<CopyPhysicalDirectoryFullPathContainingFolderCommand>
{
    protected override async Task ExecuteCoreAsync(PhysicalDirectoryModel physicalDirectory)
    {
        await ClipboardHelper.SetTextAsync(physicalDirectory.FullPath ?? "",
            "Physical directory full path copied to clipboard");
    }
}