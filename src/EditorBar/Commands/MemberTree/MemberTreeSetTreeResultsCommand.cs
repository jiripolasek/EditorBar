// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarMemberTreePopupMenu_TreeResultsCommand)]
[UsedImplicitly]
internal sealed class MemberTreeSetTreeResultsCommand
    : BaseMemberTreeResultsMenuCommand<MemberTreeSetTreeResultsCommand>
{
    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = this.GetActiveContext()?.Owner.PrefersListResultsView == false;
    }

    protected override Task ExecuteCoreAsync(MemberTreeResultsMenuContext context)
    {
        context.Owner.SetPreferredResultsView(preferList: false);
        return Task.CompletedTask;
    }
}
