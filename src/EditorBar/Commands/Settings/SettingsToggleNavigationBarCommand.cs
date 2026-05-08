// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using Community.VisualStudio.Toolkit;
using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Helpers;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Commands;

[Command(PackageGuids.EditorBarCmdSetString, PackageIds.EditorBarSettings_ToggleNavigationBarCommand)]
[UsedImplicitly]
internal sealed class SettingsToggleNavigationBarCommand
    : BaseMenuContextCommand<SettingsMenuContext, SettingsToggleNavigationBarCommand>
{
    private IMenuContextService? _menuContextService;

    protected override async Task InitializeCompletedAsync()
    {
        await base.InitializeCompletedAsync();
        this._menuContextService = await this.Package.GetServiceAsync<IMenuContextService, IMenuContextService>();
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        base.BeforeQueryStatus(e);

        var context = this._menuContextService?.GetActiveContext<SettingsMenuContext>(
            new MenuId(PackageGuids.EditorBarCmdSet, PackageIds.EditorBarSettingsMenu));

        if (context?.CurrentTextView != null)
        {
            var isEnabled = NavigationBarHelper.IsNavigationBarEnabled(context.CurrentTextView);
            this.Command.Checked = isEnabled ?? false;
        }
    }

    protected override async Task ExecuteCoreAsync(SettingsMenuContext context)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        if (context.CurrentTextView != null)
        {
            NavigationBarHelper.ToggleNavigationBar(context.CurrentTextView);
        }
    }
}
