// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// A Visual Studio command that opens Windows Explorer and selects the current file.
/// </summary>
/// <seealso cref="BaseFileActionMenuContextCommand{TCommand}" />
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_OpenContainingFolderCommand)]
internal sealed class OpenContainingFolderCommand : BaseFileActionMenuContextCommand<OpenContainingFolderCommand>
{
    protected override Task ExecuteCoreAsync(string filePath)
    {
        Launcher.OpenContaingFolder(filePath);
        return Task.CompletedTask;
    }
}
