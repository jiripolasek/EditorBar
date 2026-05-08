// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Options;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSettings_OpenOptionsCommand)]
[UsedImplicitly]
internal sealed class SettingsOpenOptionsCommand
    : BaseMenuContextCommand<SettingsMenuContext, SettingsOpenOptionsCommand>
{
    protected override async Task ExecuteCoreAsync(SettingsMenuContext context)
    {
        await VS.Settings.OpenAsync<GeneralOptionPage>();
    }
}
