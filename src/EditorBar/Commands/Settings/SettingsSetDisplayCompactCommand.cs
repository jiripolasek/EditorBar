// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Options;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSettings_DisplayCompactCommand)]
[UsedImplicitly]
internal sealed class SettingsSetDisplayCompactCommand
    : BaseMenuContextCommand<SettingsMenuContext, SettingsSetDisplayCompactCommand>
{
    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = GeneralOptionsModel.Instance.DisplayStyle == DisplayStyle.Compact;
    }

    protected override async Task ExecuteCoreAsync(SettingsMenuContext context)
    {
        GeneralOptionsModel.Instance.DisplayStyle = DisplayStyle.Compact;
        await GeneralOptionsModel.Instance.SaveAsync();
    }
}
