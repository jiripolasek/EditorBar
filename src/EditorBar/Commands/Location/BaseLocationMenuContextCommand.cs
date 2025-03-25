// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Services.LocationProviders;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

internal abstract class BaseLocationMenuContextCommand<TCommand>
    : BaseMenuContextCommand<LocationBreadcrumbMenuContext, TCommand> where TCommand : class, new()
{
    protected override Task ExecuteCoreAsync(LocationBreadcrumbMenuContext context)
    {
        return this.ExecuteCoreAsync(context.CurrentProject!, context.CurrentTextView!);
    }

    protected abstract Task ExecuteCoreAsync(IProjectInfo project, IWpfTextView wpfTextView);
}