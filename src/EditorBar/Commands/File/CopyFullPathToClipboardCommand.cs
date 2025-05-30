﻿// ------------------------------------------------------------
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
/// A Visual Studio command that opens the copies the full path of the current document to Clipboard.
/// </summary>
/// <seealso cref="BaseFileActionMenuContextCommand{TCommand}" />
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileAction_CopyFullPathCommand)]
internal sealed class CopyFullPathToClipboardCommand
    : BaseFileActionMenuContextCommand<CopyFullPathToClipboardCommand>
{
    protected override Task ExecuteCoreAsync(ITextDocument currentDocument)
    {
        Launcher.CopyAbsolutePath(currentDocument.FilePath);
        return Task.CompletedTask;
    }
}