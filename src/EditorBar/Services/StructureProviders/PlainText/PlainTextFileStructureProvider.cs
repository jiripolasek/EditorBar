// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Collections.Immutable;
using System.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.StructureProviders.PlainText;

internal class PlainTextFileStructureProvider : StructuredDocumentStructureProvider<object>
{
    public PlainTextFileStructureProvider(ITextView textView) : base(textView) { }

    protected override Task<object> ParseDocumentAsync(string text, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(null!);
    }

    protected override Task<(IEnumerable<BaseStructureModel> structure, bool rootHasChildren)>
        GetFileStructureCoreAsync(
            int caretPosition,
            object? document,
            ITextSnapshot textSnapshot,
            string filePath,
            CancellationToken cancellationToken)
    {
        return Task.FromResult<(IEnumerable<BaseStructureModel> structure, bool rootHasChildren)>((
            ImmutableList<BaseStructureModel>.Empty, false));
    }

    protected override Task<ImmutableList<FileStructureElementModel>> GetChildItemsCoreAsync(
        BaseStructureModel parentModel,
        object? document,
        ITextSnapshot textSnapshot,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(ImmutableList<FileStructureElementModel>.Empty);
    }
}