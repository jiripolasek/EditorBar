// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reflection;
using System.Windows.Forms;
using Community.VisualStudio.Toolkit;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace JPSoftworks.EditorBar.Commands.Abstractions;

/// <summary>
/// Service that manages active contexts for menus
/// </summary>
internal class MenuContextService : IMenuContextService
{
    private readonly Dictionary<MenuId, MenuContext> _activeContexts = [];

    /// <summary>
    /// Gets the active context for a specific menu
    /// </summary>
    public T? GetActiveContext<T>(MenuId menuId) where T : MenuContext
    {
        if (this._activeContexts.TryGetValue(menuId, out var context) && context is T typedContext)
        {
            return typedContext;
        }

        return null;
    }

    /// <summary>
    /// Shows a context menu
    /// </summary>
    public async Task ShowMenuAsync(MenuContext context)
    {
        Requires.NotNull(context, nameof(context));

        await ThreadHelper.JoinableTaskFactory!.SwitchToMainThreadAsync();

        this.SetActiveContext(context);
        try
        {
            var point = Cursor.Position;
            var shell = await VS.Services.GetUIShellAsync();
            var menuId = context.MenuId;

            POINTS[] locationPoints = [new() { x = (short)point.X, y = (short)point.Y }];
            _ = shell.ShowContextMenu(0, menuId.PackageGuid, menuId.CommandId, locationPoints, null!);
        }
        finally
        {
            this.ClearActiveContext(context.MenuId);
        }
    }

    /// <summary>
    /// Sets the active context
    /// </summary>
    private void SetActiveContext(MenuContext context)
    {
        var menuIdAttr = context.GetType().GetCustomAttribute<MenuIdAttribute>();
        if (menuIdAttr == null)
        {
            new Exception($"Context type {nameof(MenuContext)} missing MenuId attribute").Log();
            return;
        }

        var menuId = menuIdAttr.MenuId;
        this._activeContexts[menuId] = context;
    }

    /// <summary>
    /// Clears the active context for a specific menu
    /// </summary>
    private void ClearActiveContext(MenuId menuId)
    {
        this._activeContexts.Remove(menuId);
    }
}