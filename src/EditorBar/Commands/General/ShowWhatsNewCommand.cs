// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Services;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.ShowWhatsNewCommand)]
[UsedImplicitly]
internal sealed class ShowWhatsNewCommand : BaseCommand<ShowWhatsNewCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await WhatsNewService.ShowAsync();
    }
}
