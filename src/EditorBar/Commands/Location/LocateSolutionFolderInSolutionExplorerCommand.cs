// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSolutionFolderMenu_LocateInSolutionExplorerCommand)]
internal sealed class LocateSolutionFolderInSolutionExplorerCommand
    : BaseMenuContextCommand<SolutionFolderBreadcrumbMenuContext, LocateSolutionFolderInSolutionExplorerCommand>
{
    protected override async Task ExecuteCoreAsync(SolutionFolderBreadcrumbMenuContext context)
    {
        await context.CurrentSolutionFolder.SelectInSolutionExplorerAsync();
    }
}
