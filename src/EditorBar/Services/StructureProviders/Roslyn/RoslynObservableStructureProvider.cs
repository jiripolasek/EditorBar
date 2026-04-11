// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Microsoft;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.StructureProviders.Roslyn;

/// <summary>
/// Provides observable structure for the source files supported by Roslyn.
/// </summary>
internal sealed class RoslynObservableStructureProvider : BaseStructureProvider
{
    private readonly CompositeDisposable _disposables = [];
    private readonly RoslynWorkspaceFileStructureProvider _legacyStructureProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoslynObservableStructureProvider" /> class.
    /// </summary>
    /// <param name="textView">The text view associated with the structure provider.</param>
    public RoslynObservableStructureProvider(ITextView textView)
        : base(textView)
    {
        Requires.NotNull(textView);

        this._legacyStructureProvider = new RoslynWorkspaceFileStructureProvider(textView);

        this.UnifiedSource.Select(async _ =>
            {
                try
                {
                    this.BreadcrumbsSource.OnNext(await this._legacyStructureProvider.GetFileStructureAsync());
                }
                catch (Exception ex)
                {
                    await ex.LogAsync();
                }
            })
            .Subscribe()
            .AddTo(this._disposables);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this._disposables.Dispose();
        base.Dispose();
    }

    /// <inheritdoc />
    public override Task<ImmutableList<FileStructureElementModel>> GetChildItemsAsync(
        BaseStructureModel parentModel,
        CancellationToken cancellationToken)
    {
        return this._legacyStructureProvider.GetChildItemsAsync(parentModel);
    }
}
