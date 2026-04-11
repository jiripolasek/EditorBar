// ------------------------------------------------------------
// Copyright (c) Jiří Polášek. All rights reserved.
// ------------------------------------------------------------

#nullable enable

using System.Reactive.Linq;
using System.Windows.Threading;
using JPSoftworks.EditorBar.Helpers;
using Microsoft;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

internal abstract class BaseStructureProvider : StructureProvider
{
    private readonly ITextBuffer _textBuffer;

    protected IObservable<SnapshotPoint> CaretPositionChanged { get; }

    protected IObservable<SnapshotPoint> ContentChanged { get; }

    protected IObservable<string> DocumentNameChanged { get; }

    protected ITextView TextView { get; }

    protected IObservable<SnapshotPoint> UnifiedSource { get; }

    protected BaseStructureProvider(ITextView textView)
    {
        Requires.NotNull(textView);

        this.TextView = textView;
        this._textBuffer = textView.TextBuffer!;

        var textDocument = this.TextView.GetTextDocumentFromDocumentBuffer()!;

        var fileActionOccurred = Observable.FromEventPattern<TextDocumentFileActionEventArgs>(
                handler => textDocument.FileActionOccurred += handler,
                handler => textDocument.FileActionOccurred -= handler)
            .Where(static t => t.EventArgs.FileActionType.HasFlag(FileActionTypes.DocumentRenamed))
            .Select(static e => e.EventArgs)
            .ObserveOn(Dispatcher.CurrentDispatcher)
            .LogAndRetry("fileActionOccurred");

        this.DocumentNameChanged = fileActionOccurred
            .Select(static e => e.FilePath)
            .StartWith(textDocument.FilePath)
            .LogAndRetry("DocumentNameChanged");

        this.CaretPositionChanged = Observable.FromEventPattern<CaretPositionChangedEventArgs>(
                handler => this.TextView.Caret!.PositionChanged += handler,
                handler => this.TextView.Caret!.PositionChanged -= handler)
            .Throttle(TimeSpan.FromMilliseconds(50))
            .Select(static e => e.EventArgs.NewPosition.BufferPosition)
            .LogAndRetry("CaretPositionChanged");

        this.ContentChanged = Observable.FromEventPattern(
                handler => this._textBuffer.PostChanged += handler,
                handler => this._textBuffer.PostChanged -= handler)
            .Where(_ => this.TextView.Caret != null)
            .Select(_ => this.TextView.Caret!.Position.BufferPosition)
            .Throttle(TimeSpan.FromMilliseconds(250))
            .LogAndRetry("ContentChanged");

        this.UnifiedSource = this.CaretPositionChanged
            .Merge(this.ContentChanged)
            .StartWith(this.TextView.Caret?.Position.BufferPosition ?? default)
            .LogAndRetry("UnifiedSource");
    }
}
