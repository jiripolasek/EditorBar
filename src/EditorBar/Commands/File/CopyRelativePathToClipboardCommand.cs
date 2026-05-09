// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// A Visual Studio command that opens the copies the relative path (relative to solution root) of the current document to
/// Clipboard.
/// </summary>
/// <seealso cref="BaseFileActionMenuContextCommand{TCommand}" />
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_CopyRelativePathCommand)]
internal sealed class CopyRelativePathToClipboardCommand
    : BaseFileActionMenuContextCommand<CopyRelativePathToClipboardCommand>
{
    protected override async Task ExecuteCoreAsync(string filePath)
    {
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();
        Launcher.CopyRelativePathFromFullPath(filePath);
    }
}
