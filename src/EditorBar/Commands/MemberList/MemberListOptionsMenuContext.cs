// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Drawing;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Controls;

namespace JPSoftworks.EditorBar.Commands;

[MenuId(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarMemberListPopupMenu)]
internal sealed record MemberListOptionsMenuContext(
    MemberList Owner,
    Point? MenuLocation = null)
    : MenuContext
{
    public override Point? ScreenLocation => MenuLocation;

    public override bool Validate()
    {
        return this.Owner != null;
    }
}
