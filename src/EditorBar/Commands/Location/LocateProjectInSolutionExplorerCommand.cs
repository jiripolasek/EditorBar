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

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarProjectBreadcrumbMenu_LocateInSolutionExplorerCommand)]
internal sealed class
    LocateProjectInSolutionExplorerCommand : BaseLocationMenuContextCommand<
    LocateProjectInSolutionExplorerCommand>
{
    protected override async Task ExecuteCoreAsync(IProjectInfo project, IWpfTextView wpfTextView)
    {
        if (project is GenericProjectInfo projectWrapper)
        {
            await projectWrapper.Project.SelectInSolutionExplorerAsync();
        }
        else
        {
            await VS.StatusBar.ShowMessageAsync("Can't show properties for this project");
        }
    }
}