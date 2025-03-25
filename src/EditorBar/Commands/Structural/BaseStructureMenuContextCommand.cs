// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Commands.Abstractions;
using JPSoftworks.EditorBar.Services.StructureProviders;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
internal abstract class BaseStructureMenuContextCommand<T>
    : BaseMenuContextCommand<StructureBreadcrumbMenuContext, T> where T : class, new()
{
    protected override Task ExecuteCoreAsync(StructureBreadcrumbMenuContext context)
    {
        return this.ExecuteCoreAsync(context.CurrentBreadcrumb!, context.CurrentTextView!);
    }

    /// <summary>
    /// Executes the core asynchronous.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task ExecuteCoreAsync(BaseStructureModel baseStructureModel, IWpfTextView wpfTextView);
}