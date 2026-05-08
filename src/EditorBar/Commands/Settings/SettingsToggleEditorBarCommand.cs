// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Options;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSettings_ToggleEditorBarCommand)]
[UsedImplicitly]
internal sealed class SettingsToggleEditorBarCommand
    : BaseMenuContextCommand<SettingsMenuContext, SettingsToggleEditorBarCommand>
{
    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = GeneralOptionsModel.Instance.Enabled;
    }

    protected override async Task ExecuteCoreAsync(SettingsMenuContext context)
    {
        GeneralOptionsModel.Instance.Enabled = !GeneralOptionsModel.Instance.Enabled;
        await GeneralOptionsModel.Instance.SaveAsync();
    }
}
