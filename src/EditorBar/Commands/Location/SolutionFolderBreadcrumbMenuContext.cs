// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

[MenuId(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSolutionFolderMenu)]
internal sealed record SolutionFolderBreadcrumbMenuContext(
    SolutionItem CurrentSolutionFolder,
    IWpfTextView? CurrentTextView) : MenuContext
{
    public override bool Validate()
    {
        return this.CurrentSolutionFolder != null;
    }
}
