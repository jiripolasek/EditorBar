// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.ToggleEditorBarCommand)]
[UsedImplicitly]
internal sealed class ToggleEditorBarCommand : BaseCommand<ToggleEditorBarCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        GeneralOptionsModel.Instance.Enabled = !GeneralOptionsModel.Instance.Enabled;
        await GeneralOptionsModel.Instance.SaveAsync();
    }

    protected override Task InitializeCompletedAsync()
    {
        this.Command.Checked = GeneralOptionsModel.Instance.Enabled;
        return base.InitializeCompletedAsync();
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = GeneralOptionsModel.Instance.Enabled;
    }
}