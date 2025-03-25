// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

namespace JPSoftworks.EditorBar.Commands.Abstractions;

/// <summary>
/// Defines the menu ID for a context menu
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
internal class MenuIdAttribute : Attribute
{
    public MenuId MenuId { get; }

    public MenuIdAttribute(string packageGuidString, int commandId) : this(Guid.Parse(packageGuidString), commandId) { }

    public MenuIdAttribute(Guid packageGuid, int commandId)
    {
        this.MenuId = new MenuId(packageGuid, commandId);
    }
}