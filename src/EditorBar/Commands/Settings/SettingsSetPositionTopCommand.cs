// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Options;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSettings_PositionTopCommand)]
[UsedImplicitly]
internal sealed class SettingsSetPositionTopCommand
    : BaseMenuContextCommand<SettingsMenuContext, SettingsSetPositionTopCommand>
{
    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = GeneralOptionsModel.Instance.BarPosition == BarPosition.Top;
    }

    protected override async Task ExecuteCoreAsync(SettingsMenuContext context)
    {
        GeneralOptionsModel.Instance.BarPosition = BarPosition.Top;
        await GeneralOptionsModel.Instance.SaveAsync();
    }
}
