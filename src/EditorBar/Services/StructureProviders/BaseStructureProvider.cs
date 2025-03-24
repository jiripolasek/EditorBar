// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
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

    protected readonly IObservable<SnapshotPoint> CaretPositionChanged;
    protected readonly IObservable<SnapshotPoint> ContentChanged;
    protected readonly IObservable<string> DocumentNameChanged;

    protected readonly ITextView TextView;
    protected readonly IObservable<SnapshotPoint> UnifiedSource;

    protected BaseStructureProvider(ITextView textView)
    {
        Requires.NotNull(textView, nameof(textView));

        this.TextView = textView;
        this._textBuffer = textView.TextBuffer!;

        var textDocument = this.TextView.GetTextDocumentFromDocumentBuffer()!;

        var fileActionOccurred = Observable.FromEventPattern<TextDocumentFileActionEventArgs>(
                handler => textDocument.FileActionOccurred += handler,
                handler => textDocument.FileActionOccurred -= handler)
            .Where(static t => t.EventArgs.FileActionType.HasFlag(FileActionTypes.DocumentRenamed))
            .Select(static e => e.EventArgs)
            .ObserveOn(Dispatcher.CurrentDispatcher)
            .LogAndRetry();

        this.DocumentNameChanged = fileActionOccurred
            .Select(static e => e.FilePath)
            .StartWith(textDocument.FilePath)
            .LogAndRetry();

        this.CaretPositionChanged = Observable.FromEventPattern<CaretPositionChangedEventArgs>(
                handler => this.TextView.Caret!.PositionChanged += handler,
                handler => this.TextView.Caret!.PositionChanged -= handler)
            .Throttle(TimeSpan.FromMilliseconds(50))
            .Select(static e => e.EventArgs.NewPosition.BufferPosition)
            .LogAndRetry();

        this.ContentChanged = Observable.FromEventPattern(
                handler => this._textBuffer.PostChanged += handler,
                handler => this._textBuffer.PostChanged -= handler)
            .Where(_ => this.TextView.Caret != null)
            .Select(_ => this.TextView.Caret!.Position.BufferPosition)
            .Throttle(TimeSpan.FromMilliseconds(250))
            .LogAndRetry();

        this.UnifiedSource = this.CaretPositionChanged
            .Merge(this.ContentChanged)
            .StartWith(this.TextView.Caret?.Position.BufferPosition ?? new SnapshotPoint())
            .LogAndRetry();
    }
}