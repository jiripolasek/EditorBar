using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar;

public abstract class BaseEditorBarFactory : IWpfTextViewMarginProvider
{
    private BarPosition TargetBarPosition { get; }

    private bool _shouldBeVisible;
    private IWpfTextView? _textView;
    private IWpfTextViewMargin? _currentMargin;

    protected BaseEditorBarFactory(BarPosition targetBarPosition)
    {
        TargetBarPosition = targetBarPosition;
        GeneralPage.Saved += GeneralPageOnSaved;
        _shouldBeVisible = GeneralPage.Instance.BarPosition == TargetBarPosition;
    }

    private void GeneralPageOnSaved(GeneralPage optionsPage)
    {
        _currentMargin?.Dispose();

        if (!_shouldBeVisible && optionsPage.BarPosition == TargetBarPosition)
        {
            // recreate margin
            _shouldBeVisible = true;
            if (_textView != null)
            {
                RefreshTextView(_textView);
            }
        }
        _shouldBeVisible = optionsPage.BarPosition == TargetBarPosition;
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
        _shouldBeVisible = GeneralPage.Instance.BarPosition == TargetBarPosition;
        _textView = wpfTextViewHost.TextView;
        _currentMargin = _shouldBeVisible ? new EditorBarControl(_textView) : null;
        return _currentMargin;
    }
}