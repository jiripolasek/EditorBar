// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_LocateInSolutionExplorer)]
[UsedImplicitly]
internal sealed class LocateInSolutionExplorerCommand
    : BaseFileActionMenuContextCommand<LocateInSolutionExplorerCommand>
{
    protected override Task ExecuteCoreAsync(string filePath)
    {
        return !string.IsNullOrWhiteSpace(filePath)
            ? ProjectProperties.SelectInSolutionExplorerAsync(filePath)
            : Task.CompletedTask;
    }
}
