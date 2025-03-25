// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_LocateInSolutionExplorer)]
[UsedImplicitly]
internal sealed class LocateInSolutionExplorerCommand
    : BaseFileActionMenuContextCommand<LocateInSolutionExplorerCommand>
{
    protected override Task ExecuteCoreAsync(ITextDocument currentDocument)
    {
        return !string.IsNullOrWhiteSpace(currentDocument.FilePath!)
            ? ProjectProperties.SelectInSolutionExplorerAsync(currentDocument.FilePath!)
            : Task.CompletedTask;
    }
}