// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Options;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSettings_PositionBottomCommand)]
[UsedImplicitly]
internal sealed class SettingsSetPositionBottomCommand
    : BaseMenuContextCommand<SettingsMenuContext, SettingsSetPositionBottomCommand>
{
    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = GeneralOptionsModel.Instance.BarPosition == BarPosition.Bottom;
    }

    protected override async Task ExecuteCoreAsync(SettingsMenuContext context)
    {
        GeneralOptionsModel.Instance.BarPosition = BarPosition.Bottom;
        await GeneralOptionsModel.Instance.SaveAsync();
    }
}
