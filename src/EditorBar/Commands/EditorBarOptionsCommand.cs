// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.guidEditorBarPackageCmdSetString, PackageIds.EditorBarOptionsCommand)]
internal sealed class EditorBarOptionsCommand : BaseCommand<EditorBarOptionsCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.Settings.OpenAsync<GeneralOptionPage>();
    }
}