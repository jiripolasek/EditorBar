// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Services;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarStructureBreadcrumbMenuNavigateToCodeElementCommand)]
internal sealed class NavigateToCodeElementCommand : BaseStructureMenuContextCommand<NavigateToCodeElementCommand>
{
    protected override async Task ExecuteCoreAsync(BaseStructureModel baseStructureModel, IWpfTextView wpfTextView)
    {
        await Navigator.NavigateToAnchorAsync(baseStructureModel.AnchorPoint);
    }
}