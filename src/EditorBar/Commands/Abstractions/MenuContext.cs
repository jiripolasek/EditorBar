// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reflection;

namespace JPSoftworks.EditorBar.Commands.Abstractions;

/// <summary>
/// Base record for all menu context objects.
/// </summary>
internal abstract record MenuContext
{
    /// <summary>
    /// Gets the id of menu associated with this context.
    /// </summary>
    public virtual MenuId MenuId =>
        this.GetType().GetCustomAttribute<MenuIdAttribute>()?.MenuId
        ?? throw new InvalidOperationException($"Context type {this.GetType().Name} is missing MenuId attribute");

    /// <summary>
    /// Validates the context before use.
    /// </summary>
    /// <returns>Returns <c>true</c> if menu context parametersa are valid and menu can be shown.</returns>
    public virtual bool Validate()
    {
        return true;
    }
}