// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Services.LocationProviders;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

/// <summary>
/// Opens a terminal rooted at the selected project or solution location.
/// </summary>
[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarProjectBreadcrumbMenu_OpenTerminalCommand)]
internal sealed class OpenProjectInTerminalCommand
    : BaseLocationMenuContextCommand<OpenProjectInTerminalCommand>
{
    protected override Task ExecuteCoreAsync(IProjectInfo project, IWpfTextView wpfTextView)
    {
        Launcher.OpenTerminal(ProjectLocationHelper.GetProjectLaunchPath(project));
        return Task.CompletedTask;
    }
}
