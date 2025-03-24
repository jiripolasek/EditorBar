// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reflection;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Commands.Abstractions;

/// <summary>
/// Base class for commands that use a specific context type
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
internal abstract class BaseMenuContextCommand<TMenuContext, TCommand> : BaseCommand<TCommand>
    where TMenuContext : MenuContext
    where TCommand : class, new()
{
    private IMenuContextService? _contextService;

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        try
        {
            this._contextService ??= await this.Package.GetServiceAsync<IMenuContextService, IMenuContextService>();

            var menuIdAttr = typeof(TMenuContext).GetCustomAttribute<MenuIdAttribute>();
            if (menuIdAttr == null)
            {
                await new Exception($"Context type {typeof(TMenuContext).Name} missing MenuId attribute").LogAsync();
                return;
            }

            var menuId = menuIdAttr.MenuId;
            var context = this._contextService?.GetActiveContext<TMenuContext>(menuId);
            if (context == null || context.Validate() == false)
            {
                return;
            }

            await this.ExecuteCoreAsync(context);
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }
    }

    protected abstract Task ExecuteCoreAsync(TMenuContext context);
}