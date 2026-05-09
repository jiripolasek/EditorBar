// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// A Visual Studio command that opens the current document in an external editor.
/// </summary>
/// <seealso cref="BaseFileActionMenuContextCommand{TCommand}" />
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_OpenFileInExternalEditorCommand)]
internal sealed class OpenInExternalEditorCommand : BaseFileActionMenuContextCommand<OpenInExternalEditorCommand>
{
    protected override Task ExecuteCoreAsync(string filePath)
    {
        Launcher.OpenInExternalEditor(filePath);
        return Task.CompletedTask;
    }
}
