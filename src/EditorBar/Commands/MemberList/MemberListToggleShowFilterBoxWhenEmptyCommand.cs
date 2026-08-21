// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarMemberListPopupMenu_ShowFilterBoxWhenEmptyCommand)]
[UsedImplicitly]
internal sealed class MemberListToggleShowFilterBoxWhenEmptyCommand
    : BaseMemberListOptionsMenuCommand<MemberListToggleShowFilterBoxWhenEmptyCommand>
{
    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = this.GetActiveContext()?.Owner.ShowFilterBoxWhenEmpty == true;
    }

    protected override Task ExecuteCoreAsync(MemberListOptionsMenuContext context)
    {
        context.Owner.SetShowFilterBoxWhenEmpty(!context.Owner.ShowFilterBoxWhenEmpty);
        return Task.CompletedTask;
    }
}
