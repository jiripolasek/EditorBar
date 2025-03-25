// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Threading;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

/// <summary>
/// A structure provider that does not generate breadcrumbs or child items.
/// </summary>
/// <remarks>Use this as a fallback when no structure data is available.</remarks>
internal class NullBreadcrumbProvider : IStructureProvider
{
    /// <summary>
    /// Gets the shared instance of <see cref="NullBreadcrumbProvider" />.
    /// </summary>
    public static NullBreadcrumbProvider Instance { get; } = new();

    /// <summary>
    /// Gets an observable that notifies subscribers about the current breadcrumb structure.
    /// </summary>
    public IObservable<StructureNavModel> BreadcrumbsChanged => Observable.Return(new StructureNavModel(false, []));

    /// <summary>
    /// Retrieves an empty collection of child items for any given parent model.
    /// </summary>
    /// <param name="parentModel">The parent model.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An empty list of file structure elements.</returns>
    public Task<ImmutableList<FileStructureElementModel>> GetChildItemsAsync(
        BaseStructureModel parentModel,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(ImmutableList<FileStructureElementModel>.Empty);
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}