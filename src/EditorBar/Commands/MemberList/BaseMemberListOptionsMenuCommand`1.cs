// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Commands;

internal abstract class BaseMemberListOptionsMenuCommand<TCommand>
    : BaseMenuContextCommand<MemberListOptionsMenuContext, TCommand>
    where TCommand : class, new()
{
    private IMenuContextService? _menuContextService;

    protected override async Task InitializeCompletedAsync()
    {
        await base.InitializeCompletedAsync();
        this._menuContextService = await this.Package.GetServiceAsync<IMenuContextService, IMenuContextService>();
    }

    protected MemberListOptionsMenuContext? GetActiveContext()
    {
        return this._menuContextService?.GetActiveContext<MemberListOptionsMenuContext>(
            new MenuId(PackageGuids.EditorBarCmdSet, PackageIds.EditorBarMemberListPopupMenu));
    }
}
