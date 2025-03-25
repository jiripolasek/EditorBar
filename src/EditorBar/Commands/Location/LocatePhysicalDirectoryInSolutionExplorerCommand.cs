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

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarPhysicalDirectoryMenu_LocateInSolutionExplorerCommand)]
internal sealed class LocatePhysicalDirectoryInSolutionExplorerCommand :
    BasePhysicalLocationContextMenuCommand<LocatePhysicalDirectoryInSolutionExplorerCommand>
{
    protected override async Task ExecuteCoreAsync(PhysicalDirectoryModel physicalDirectory)
    {
        await ProjectProperties.SelectInSolutionExplorerAsync(physicalDirectory.FullPath);
    }
}