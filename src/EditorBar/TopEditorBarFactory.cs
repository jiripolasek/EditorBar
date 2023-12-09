// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.ComponentModel.Composition;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace JPSoftworks.EditorBar;

[Export(typeof(IWpfTextViewMarginProvider))]
[Name("EditorBar-Top")]
[Order(After = PredefinedMarginNames.HorizontalScrollBar)]
[MarginContainer(PredefinedMarginNames.Top)]
[ContentType(StandardContentTypeNames.Text)]
[TextViewRole(PredefinedTextViewRoles.Document)]
[DeferCreation(OptionName = DefaultTextViewHostOptions.EditingStateMarginOptionName)]
public class TopEditorBarFactory : IWpfTextViewMarginProvider
{
    private bool _isTop;
    private IWpfTextView? _textView;
    private IWpfTextViewMargin? _currentMargin;

    public TopEditorBarFactory()
    {
        GeneralPage.Saved += GeneralPageOnSaved;
    }

    private void GeneralPageOnSaved(GeneralPage obj)
    {
        _currentMargin?.Dispose();

        if (!_isTop && obj.BarPosition == BarPosition.Top)
        {
            // recreate margin
            _isTop = true;
            if (_textView != null)
            {
                RefreshTextView(_textView);
            }
        }

        _isTop = obj.BarPosition == BarPosition.Top;
    }

    public IWpfTextViewMargin? CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
    {
        _isTop = GeneralPage.Instance.BarPosition == BarPosition.Top;
        _textView = wpfTextViewHost.TextView;
        _currentMargin = _isTop ? new EditorBarControl(_textView) : null;
        return _currentMargin;
    }

    private void RefreshTextView(IWpfTextView textView)
    {
        textView.DisplayTextLineContainingBufferPosition(new SnapshotPoint(textView.TextSnapshot, 0), 0, ViewRelativePosition.Top);

        var originalZoomLevel = textView.ZoomLevel;
        textView.ZoomLevel = originalZoomLevel + 0.01;  // Slight zoom change
        textView.ZoomLevel = originalZoomLevel;         // Revert the zoom change
    }
}