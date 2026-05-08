// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Options;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSettings_DisplayNormalCommand)]
[UsedImplicitly]
internal sealed class SettingsSetDisplayNormalCommand
    : BaseMenuContextCommand<SettingsMenuContext, SettingsSetDisplayNormalCommand>
{
    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = GeneralOptionsModel.Instance.DisplayStyle == DisplayStyle.Normal;
    }

    protected override async Task ExecuteCoreAsync(SettingsMenuContext context)
    {
        GeneralOptionsModel.Instance.DisplayStyle = DisplayStyle.Normal;
        await GeneralOptionsModel.Instance.SaveAsync();
    }
}
