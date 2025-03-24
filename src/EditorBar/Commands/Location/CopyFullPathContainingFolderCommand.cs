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

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarProjectBreadcrumbMenu_CopyProjectFullPathCommand)]
internal sealed class
    CopyFullPathContainingFolderCommand : BaseLocationMenuContextCommand<
    CopyFullPathContainingFolderCommand>
{
    protected override async Task ExecuteCoreAsync(IProjectInfo project, IWpfTextView wpfTextView)
    {
        var path = project is GenericProjectInfo projectWrapper &&
                   !string.IsNullOrWhiteSpace(projectWrapper.Project.FullPath!)
            ? projectWrapper.Project.FullPath
            : project.DirectoryPath;
        await ClipboardHelper.SetTextAsync(path, "Project full path copied to clipboard");
    }
}