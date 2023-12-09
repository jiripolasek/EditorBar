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
[Name("EditorBar-Bottom")]
[Order(After = PredefinedMarginNames.HorizontalScrollBar)]
[MarginContainer(PredefinedMarginNames.Bottom)]
[ContentType(StandardContentTypeNames.Text)]
[TextViewRole(PredefinedTextViewRoles.Document)]
public class BottomEditorBarFactory : IWpfTextViewMarginProvider
{
    private bool _isBottom;
    private IWpfTextView? _textView;
    private IWpfTextViewMargin? _currentMargin;

    public BottomEditorBarFactory()
    {
        GeneralPage.Saved += GeneralPageOnSaved;
        _isBottom = GeneralPage.Instance.BarPosition == BarPosition.Bottom;
    }

    private void GeneralPageOnSaved(GeneralPage obj)
    {
        _currentMargin?.Dispose();

        if (!_isBottom && obj.BarPosition == BarPosition.Bottom)
        {
            // recreate margin
            _isBottom = true;
            if (_textView != null)
            {
                RefreshTextView(_textView);
            }
        }
        _isBottom = obj.BarPosition == BarPosition.Bottom;
    }

    private static void RefreshTextView(IWpfTextView textView)
    {
        textView.DisplayTextLineContainingBufferPosition(new SnapshotPoint(textView.TextSnapshot, 0), 0, ViewRelativePosition.Top);

        var originalZoomLevel = textView.ZoomLevel;
        textView.ZoomLevel = originalZoomLevel + 0.01;  // Slight zoom change
        textView.ZoomLevel = originalZoomLevel;         // Revert the zoom change
    }


    public IWpfTextViewMargin? CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
    {
        _isBottom = GeneralPage.Instance.BarPosition == BarPosition.Bottom;
        _textView = wpfTextViewHost.TextView;
        _currentMargin = _isBottom ? new EditorBarControl(_textView) : null;
        return _currentMargin;
    }
}