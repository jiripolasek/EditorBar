// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Options;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSettings_PositionBottomControlCommand)]
[UsedImplicitly]
internal sealed class SettingsSetPositionBottomControlCommand
    : BaseMenuContextCommand<SettingsMenuContext, SettingsSetPositionBottomControlCommand>
{
    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = GeneralOptionsModel.Instance.BarPosition == BarPosition.BottomControl;
    }

    protected override async Task ExecuteCoreAsync(SettingsMenuContext context)
    {
        GeneralOptionsModel.Instance.BarPosition = BarPosition.BottomControl;
        await GeneralOptionsModel.Instance.SaveAsync();
    }
}
