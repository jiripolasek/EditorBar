// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
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
        // If IProjectInfo is ProjectWrapper then we can extract the FullPath property to the project open containing folder and preselect the project file;
        // Otherwise just use project.DirectoryPath to open plain folder.

        if (project is GenericProjectInfo projectWrapper
            && !string.IsNullOrWhiteSpace(projectWrapper.Project.FullPath!))
        {
            Launcher.OpenContaingFolder(projectWrapper.Project.FullPath);
            return Task.CompletedTask;
        }

        Launcher.OpenFolder(project.DirectoryPath);
        return Task.CompletedTask;
    }
}