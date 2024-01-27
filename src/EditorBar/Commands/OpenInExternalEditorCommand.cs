// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// A Visual Studio command that opens the current document in an external editor.
/// </summary>
/// <seealso cref="EditorBarFileActionMenuBridgeCommand{T}" />
[Command(PackageGuids.guidEditorBarPackageCmdSetString, PackageIds.EditorBarFileAction_OpenFileInExternalEditorCommand)]
internal sealed class OpenInExternalEditorCommand : EditorBarFileActionMenuBridgeCommand<OpenInExternalEditorCommand>
{
    protected override Task ExecuteCoreAsync(ITextDocument currentDocument)
    {
        Launcher.OpenInExternalEditor(currentDocument.FilePath);
        return Task.CompletedTask;
    }
}