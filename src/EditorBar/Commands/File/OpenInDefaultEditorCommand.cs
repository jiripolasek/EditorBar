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
/// A Visual Studio command that opens the current document in the default editor.
/// </summary>
/// <seealso cref="BaseFileActionMenuContextCommand{TCommand}" />
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_OpenFileInDefaultEditorCommand)]
internal sealed class OpenInDefaultEditorCommand : BaseFileActionMenuContextCommand<OpenInDefaultEditorCommand>
{
    protected override Task ExecuteCoreAsync(ITextDocument currentDocument)
    {
        Launcher.OpenInDefaultEditor(currentDocument.FilePath);
        return Task.CompletedTask;
    }
}