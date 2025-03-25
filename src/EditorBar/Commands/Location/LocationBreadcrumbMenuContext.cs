// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Services.LocationProviders;
using JPSoftworks.EditorBar.ViewModels;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

[MenuId(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarProjectBreadcrumbMenu)]
internal sealed record LocationBreadcrumbMenuContext(
    IProjectInfo? CurrentProject,
    PhysicalDirectoryModel? PhysicalDirectory,
    IWpfTextView? CurrentTextView) : MenuContext
{
    public override MenuId MenuId
    {
        get
        {
            if (this.CurrentProject != null)
            {
                return new MenuId(PackageGuids.EditorBarCmdSet, PackageIds.EditorBarProjectBreadcrumbMenu);
            }

            if (this.PhysicalDirectory != null)
            {
                return new MenuId(PackageGuids.EditorBarCmdSet, PackageIds.EditorBarPhysicalDirectoryMenu);
            }

            return new MenuId(Guid.Empty, -1);
        }
    }

    public LocationBreadcrumbMenuContext(IProjectInfo? currentProject, IWpfTextView? currentTextView)
        : this(currentProject, null, currentTextView)
    {
    }

    public LocationBreadcrumbMenuContext(PhysicalDirectoryModel? physicalDirectory, IWpfTextView? currentTextView)
        : this(null, physicalDirectory, currentTextView)
    {
    }
}