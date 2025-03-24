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

/// <summary>
/// A Visual Studio command that opens Windows Explorer and selects the current file.
/// </summary>
/// <seealso cref="BaseFileActionMenuContextCommand{TCommand}" />
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_OpenContainingFolderCommand)]
internal sealed class OpenContainingFolderCommand : BaseFileActionMenuContextCommand<OpenContainingFolderCommand>
{
    protected override Task ExecuteCoreAsync(ITextDocument currentDocument)
    {
        Launcher.OpenContaingFolder(currentDocument.FilePath);
        return Task.CompletedTask;
    }
}