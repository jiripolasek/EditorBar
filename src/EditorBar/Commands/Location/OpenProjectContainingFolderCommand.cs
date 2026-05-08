// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Services.LocationProviders;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarProjectBreadcrumbMenu_OpenContainingFolderCommand)]
internal sealed class OpenProjectContainingFolderCommand
    : BaseLocationMenuContextCommand<OpenProjectContainingFolderCommand>
{
    protected override Task ExecuteCoreAsync(IProjectInfo project, IWpfTextView wpfTextView)
    {
        var path = ProjectLocationHelper.GetProjectLaunchPath(project);
        if (path == null)
        {
            return Task.CompletedTask;
        }

        if (project is GenericProjectInfo)
        {
            Launcher.OpenContaingFolder(path);
            return Task.CompletedTask;
        }

        Launcher.OpenFolder(path);
        return Task.CompletedTask;
    }
}
