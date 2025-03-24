// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.ViewModels;

namespace JPSoftworks.EditorBar.Commands;

internal abstract class BasePhysicalLocationContextMenuCommand<TCommand>
    : BaseMenuContextCommand<LocationBreadcrumbMenuContext, TCommand> where TCommand : class, new()
{
    protected override Task ExecuteCoreAsync(LocationBreadcrumbMenuContext context)
    {
        return this.ExecuteCoreAsync(context.PhysicalDirectory!.Value);
    }

    protected abstract Task ExecuteCoreAsync(PhysicalDirectoryModel physicalDirectory);
}