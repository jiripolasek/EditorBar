// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using System.Reactive.Subjects;
using System.Threading;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

internal abstract class StructureProvider : IStructureProvider
{
    protected BehaviorSubject<StructureNavModel> BreadcrumbsSource { get; } = new(new StructureNavModel(false, []));

    public IObservable<StructureNavModel> BreadcrumbsChanged => this.BreadcrumbsSource;

    public abstract Task<ImmutableList<FileStructureElementModel>> GetChildItemsAsync(
        BaseStructureModel parentModel,
        CancellationToken cancellationToken);

    public virtual void Dispose()
    {
        this.BreadcrumbsSource.Dispose();
    }
}