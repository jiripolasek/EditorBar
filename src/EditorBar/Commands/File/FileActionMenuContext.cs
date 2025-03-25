// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Commands.Abstractions;
using Microsoft.VisualStudio.Text;

namespace JPSoftworks.EditorBar.Commands;

[MenuId(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarFileActionMenu)]
internal sealed record FileActionMenuContext(ITextDocument? CurrentDocument) : MenuContext
{
    public override bool Validate()
    {
        return this.CurrentDocument != null;
    }
}