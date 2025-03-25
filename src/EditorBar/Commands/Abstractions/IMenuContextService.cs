// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Commands.Abstractions;

internal interface IMenuContextService
{
    /// <summary>
    /// Gets the active context for a specific menu
    /// </summary>
    T? GetActiveContext<T>(MenuId menuId) where T : MenuContext;

    /// <summary>
    /// Shows a context menu
    /// </summary>
    Task ShowMenuAsync(MenuContext context);
}