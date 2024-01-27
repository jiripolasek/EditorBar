// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// Helper service that invokes the "File Action" menu command in context of the current document.
/// </summary>
internal class EditorBarFileActionMenuBridge
{
    internal ITextDocument? CurrentDocument { get; private set; }

    /// <summary>
    /// Helper service that invokes the "File Action" menu command in context of the current document.
    /// </summary>
    /// <param name="document">The current document.</param>
    /// <param name="x">The x-coordinate of the location where the menu should be shown.</param>
    /// <param name="y">The y-coordinate of the location where the menu should be shown.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown back at the awaiting caller if cancelled, even if the caller is already on the main thread.</exception>
    public async Task ShowAsync(ITextDocument? document, int x, int y)
    {
        if (document == null)
            return;

        // Hackish way to show the Visual Studio context menu in context of the current document.
        // - set and clear the current document to ensure that the command is not executed on a different document
        // - and pray that the command is executed on the main thread
        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();
        this.CurrentDocument = document;
        try
        {
            IVsUIShell shell = await VS.Services.GetUIShellAsync();
            POINTS[] locationPoints = [new POINTS { x = (short)x, y = (short)y }];
            _ = shell.ShowContextMenu(0, PackageGuids.guidEditorBarPackageCmdSet, PackageIds.EditorBarFileActionMenu, locationPoints, pCmdTrgtActive: null!);
        }
        finally
        {
            this.CurrentDocument = null;
        }
    }
}