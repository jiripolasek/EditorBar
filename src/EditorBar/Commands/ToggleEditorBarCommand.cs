﻿// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.guidEditorBarPackageCmdSetString, PackageIds.ToggleEditorBarCommand)]
internal sealed class ToggleEditorBarCommand : BaseCommand<ToggleEditorBarCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        GeneralOptionsModel.Instance.Enabled = !GeneralOptionsModel.Instance.Enabled;
        await GeneralOptionsModel.Instance.SaveAsync();
    }
}
