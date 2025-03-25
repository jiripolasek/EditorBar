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
/// A Visual Studio command that opens the current document in an external editor.
/// </summary>
/// <seealso cref="BaseFileActionMenuContextCommand{TCommand}" />
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_OpenFileInExternalEditorCommand)]
internal sealed class OpenInExternalEditorCommand : BaseFileActionMenuContextCommand<OpenInExternalEditorCommand>
{
    protected override Task ExecuteCoreAsync(ITextDocument currentDocument)
    {
        Launcher.OpenInExternalEditor(currentDocument.FilePath);
        return Task.CompletedTask;
    }
}