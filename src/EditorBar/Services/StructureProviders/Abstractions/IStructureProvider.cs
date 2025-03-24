// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using System.Threading;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

/// <summary>
/// Provides an interface for observable structure providers.
/// </summary>
internal interface IStructureProvider : IDisposable
{
    /// <summary>
    /// Gets the observable that notifies about the current structure model.
    /// </summary>
    IObservable<StructureNavModel> BreadcrumbsChanged { get; }

    /// <summary>
    /// Gets the child items of a given structure model.
    /// </summary>
    /// <param name="parentModel">The parent model to get child items for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an immutable list of file
    /// structure element models.
    /// </returns>
    Task<ImmutableList<FileStructureElementModel>> GetChildItemsAsync(
        BaseStructureModel parentModel,
        CancellationToken cancellationToken);
}