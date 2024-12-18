// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// A Visual Studio command that opens the copies the relative path (relative to solution root) of the current document to Clipboard.
/// </summary>
/// <seealso cref="EditorBarFileActionMenuBridgeCommand{T}" />
[Command(PackageGuids.guidEditorBarPackageCmdSetString, PackageIds.EditorBarFileAction_CopyRelativePathCommand)]
internal sealed class CopyRelativePathToClipboardCommand : EditorBarFileActionMenuBridgeCommand<CopyRelativePathToClipboardCommand>
{
    /// <exception cref="OperationCanceledException">Thrown back at the awaiting caller ifcanceled, even if the caller is already on the main thread.</exception>
    protected override async Task ExecuteCoreAsync(ITextDocument currentDocument)
    {
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();
        Launcher.CopyRelativePathFromFullPath(currentDocument.FilePath);
    }
}