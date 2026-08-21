// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarMemberTreePopupMenu_ListResultsCommand)]
[UsedImplicitly]
internal sealed class MemberTreeSetListResultsCommand
    : BaseMemberTreeResultsMenuCommand<MemberTreeSetListResultsCommand>
{
    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = this.GetActiveContext()?.Owner.PrefersListResultsView == true;
    }

    protected override Task ExecuteCoreAsync(MemberTreeResultsMenuContext context)
    {
        context.Owner.SetPreferredResultsView(preferList: true);
        return Task.CompletedTask;
    }
}
