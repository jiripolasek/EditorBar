// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

[MenuId(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarTypeBreadcrumbMenu)]
internal sealed record StructureBreadcrumbMenuContext(
    BaseStructureModel? CurrentBreadcrumb,
    IWpfTextView? CurrentTextView)
    : MenuContext
{
    public override MenuId MenuId =>
        this.CurrentBreadcrumb switch
        {
            FileModel => new MenuId(PackageGuids.EditorBarCmdSet, PackageIds.EditorBarFileActionMenu),
            TypeModel => new MenuId(PackageGuids.EditorBarCmdSet, PackageIds.EditorBarTypeBreadcrumbMenu),
            FunctionModel => new MenuId(PackageGuids.EditorBarCmdSet, PackageIds.EditorBarMemberBreadcrumbMenu),
            TypeMemberModel => new MenuId(PackageGuids.EditorBarCmdSet, PackageIds.EditorBarMemberBreadcrumbMenu),
            _ => new MenuId(Guid.Empty, -1)
        };
}