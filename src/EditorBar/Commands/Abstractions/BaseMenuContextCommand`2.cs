// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Reflection;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;

namespace JPSoftworks.EditorBar.Commands.Abstractions;

/// <summary>
/// Base class for commands that use a specific context type.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
internal abstract class BaseMenuContextCommand<TMenuContext, TCommand> : BaseCommand<TCommand>
    where TMenuContext : MenuContext
    where TCommand : class, new()
{
    private IMenuContextService? _contextService;

    protected override async Task InitializeCompletedAsync()
    {
        await base.InitializeCompletedAsync();
        this._contextService = await this.Package.GetServiceAsync<IMenuContextService, IMenuContextService>();
    }

    protected override void Execute(object sender, EventArgs e)
    {
        try
        {
            var menuIdAttr = typeof(TMenuContext).GetCustomAttribute<MenuIdAttribute>();
            if (menuIdAttr == null)
            {
                new Exception($"Context type {typeof(TMenuContext).Name} missing MenuId attribute").Log();
                return;
            }

            var menuId = menuIdAttr.MenuId;
            var context = this._contextService?.GetActiveContext<TMenuContext>(menuId);
            if (context == null || !context.Validate())
            {
                return;
            }

            // The menu service clears its active context as soon as this callback returns.
            // Capture it above before starting any asynchronous command work.
            this.ExecuteCapturedContextAsync(context).FireAndForget();
        }
        catch (Exception ex)
        {
            ex.Log();
        }
    }

    private async Task ExecuteCapturedContextAsync(TMenuContext context)
    {
        try
        {
            await this.ExecuteCoreAsync(context);
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
        }
    }

    protected abstract Task ExecuteCoreAsync(TMenuContext context);
}
