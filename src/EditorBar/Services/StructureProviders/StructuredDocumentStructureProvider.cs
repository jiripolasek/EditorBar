// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

/// <summary>
/// Provides base functionality for structure providers for structured documents.
/// </summary>
/// <typeparam name="TParsedDocument"></typeparam>
internal abstract class StructuredDocumentStructureProvider<TParsedDocument> : BaseStructureProvider
    where TParsedDocument : class
{
    private readonly CompositeDisposable _disposables = [];

    private TParsedDocument? _latestParsedDocument;
    private ITextSnapshot? _latestSnapshot;

    protected StructuredDocumentStructureProvider(ITextView textView) : base(textView)
    {
        var parsedObservable = this.UnifiedSource
            .Select(static snap => snap.Snapshot)
            .DistinctUntilChanged(static snapshot => snapshot!.Version)
            .Select(snapshot => Observable.FromAsync(async ct =>
            {
                var parsedDocument = await this.ParseFuncAsync(snapshot!, ct);
                return (parsedDocument, snapshot);
            }))
            .Switch()
            .Replay(1)
            .RefCount();

        // Subscribe to parsedObservable separately to update _latestParsedDocument.
        _ = parsedObservable.Subscribe(parsedDoc =>
            {
                this._latestParsedDocument = parsedDoc.parsedDocument;
                this._latestSnapshot = parsedDoc.snapshot;
            })
            .AddTo(this._disposables);


        var combined = this.UnifiedSource.Throttle(TimeSpan.FromMilliseconds(100)).CombineLatest(
            this.DocumentNameChanged,
            parsedObservable,
            static (snapshotPoint, documentName, parsedDocument) => new
            {
                CaretSnapshot = snapshotPoint, Path = documentName, ParsedDoc = parsedDocument
            });

        _ = combined
            .Where(static tuple => tuple.CaretSnapshot.Snapshot!.Version == tuple.ParsedDoc.snapshot!.Version)
            .Select(tuple => Observable.FromAsync(ct =>
                this.GetUpdatedBreadcrumbsAsync(tuple.CaretSnapshot, tuple.ParsedDoc.parsedDocument!, tuple.Path, ct)))
            .Switch()
            .Subscribe(this.BreadcrumbsSource.OnNext)
            .AddTo(this._disposables);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this._disposables.Dispose();
        base.Dispose();
    }

    private async Task<TParsedDocument> ParseFuncAsync(ITextSnapshot snapshot, CancellationToken cancellationToken)
    {
        return await this.ParseDocumentAsync(snapshot.GetText() ?? "", cancellationToken);
    }

    private async Task<StructureNavModel> GetUpdatedBreadcrumbsAsync(
        SnapshotPoint snapshotPoint,
        TParsedDocument parsedDocument,
        string path,
        CancellationToken cancellationToken)
    {
        var snapshot = snapshotPoint.Snapshot!;
        var caretPosition = snapshotPoint.Position;
        var breadcrumbsData
            = await this.GetFileStructureCoreAsync(caretPosition, parsedDocument, snapshot, path, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        return new StructureNavModel(true, breadcrumbsData.structure);
    }

    public override Task<ImmutableList<FileStructureElementModel>> GetChildItemsAsync(
        BaseStructureModel parentModel,
        CancellationToken cancellationToken)
    {
        if (this._latestSnapshot != null && this._latestParsedDocument != null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return this.GetChildItemsCoreAsync(parentModel, this._latestParsedDocument, this._latestSnapshot,
                cancellationToken);
        }

        return Task.FromResult(ImmutableList<FileStructureElementModel>.Empty);
    }

    /// <summary>
    /// Parses the document from the provided text.
    /// </summary>
    /// <param name="text">The text to parse.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous parse operation. The task result contains the parsed document.</returns>
    protected abstract Task<TParsedDocument> ParseDocumentAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the file structure for the specified caret position in the parsed document.
    /// </summary>
    /// <param name="caretPosition">The caret position.</param>
    /// <param name="document">The parsed document.</param>
    /// <param name="textSnapshot">The text snapshot.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a tuple with the file structure
    /// and a boolean indicating if the root has children.
    /// </returns>
    protected abstract Task<(IEnumerable<BaseStructureModel> structure, bool rootHasChildren)>
        GetFileStructureCoreAsync(
            int caretPosition,
            TParsedDocument document,
            ITextSnapshot textSnapshot,
            string filePath,
            CancellationToken cancellationToken);

    /// <summary>
    /// Gets the child items for the specified parent model.
    /// </summary>
    /// <param name="parentModel">The parent model.</param>
    /// <param name="document">The parsed document.</param>
    /// <param name="textSnapshot">The text snapshot.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of child items.</returns>
    protected abstract Task<ImmutableList<FileStructureElementModel>> GetChildItemsCoreAsync(
        BaseStructureModel parentModel,
        TParsedDocument document,
        ITextSnapshot textSnapshot,
        CancellationToken cancellationToken);
}