// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Diagnostics;

namespace JPSoftworks.EditorBar.Commands.Abstractions;

/// <summary>
/// Unique identifier for a context menu
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly record struct MenuId(Guid PackageGuid, int CommandId)
{
    private string DebuggerDisplay => $"{this.PackageGuid}: {this.CommandId:X4}";
}