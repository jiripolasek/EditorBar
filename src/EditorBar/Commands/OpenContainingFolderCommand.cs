// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// A Visual Studio command that opens Windows Explorer and selects the current file.
/// </summary>
/// <seealso cref="EditorBarFileActionMenuBridgeCommand{T}" />
[Command(PackageGuids.guidEditorBarPackageCmdSetString, PackageIds.EditorBarFileAction_OpenContainingFolderCommand)]
internal sealed class OpenContainingFolderCommand : EditorBarFileActionMenuBridgeCommand<OpenContainingFolderCommand>
{
    protected override Task ExecuteCoreAsync(ITextDocument currentDocument)
    {
        Launcher.OpenContaingFolder(currentDocument.FilePath);
        return Task.CompletedTask;
    }
}