// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarMemberTreePopupMenu_ShowFilterBoxWhenEmptyCommand)]
[UsedImplicitly]
internal sealed class MemberTreeToggleShowFilterBoxWhenEmptyCommand
    : BaseMemberTreeResultsMenuCommand<MemberTreeToggleShowFilterBoxWhenEmptyCommand>
{
    protected override void BeforeQueryStatus(EventArgs e)
    {
        base.BeforeQueryStatus(e);
        this.Command.Checked = this.GetActiveContext()?.Owner.ShowFilterBoxWhenEmpty == true;
    }

    protected override Task ExecuteCoreAsync(MemberTreeResultsMenuContext context)
    {
        context.Owner.SetShowFilterBoxWhenEmpty(!context.Owner.ShowFilterBoxWhenEmpty);
        return Task.CompletedTask;
    }
}
