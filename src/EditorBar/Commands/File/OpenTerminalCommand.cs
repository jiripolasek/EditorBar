// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// Opens a terminal rooted at the current document location.
/// </summary>
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_OpenTerminalCommand)]
internal sealed class OpenTerminalCommand : BaseFileActionMenuContextCommand<OpenTerminalCommand>
{
    protected override Task ExecuteCoreAsync(ITextDocument currentDocument)
    {
        Launcher.OpenTerminal(currentDocument.FilePath);
        return Task.CompletedTask;
    }
}
