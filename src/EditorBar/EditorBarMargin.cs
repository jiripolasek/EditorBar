// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Windows;
using JPSoftworks.EditorBar.Options;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;

namespace JPSoftworks.EditorBar;

/// <summary>
/// Represents margin for <see cref="IWpfTextView" />. It is used to display <see cref="EditorBarControl" /> in the
/// editor.
/// </summary>
public class EditorBarMargin : IWpfTextViewMargin
{
    private readonly EditorBarControl _editorBarControl;
    private readonly BarPosition _position;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorBarMargin" /> class.
    /// </summary>
    /// <param name="textView">The text view.</param>
    /// <param name="joinableTaskFactory">A factory for starting asynchronous tasks that can mitigate deadlocks.</param>
    /// <param name="position">The position.</param>
    public EditorBarMargin(IWpfTextView textView, JoinableTaskFactory joinableTaskFactory, BarPosition position)
    {
        this._position = position;
        this._editorBarControl = new EditorBarControl(textView, joinableTaskFactory);

        GeneralOptionsModel.Saved += this.GeneralPageOnSaved;
        this.GeneralPageOnSaved(GeneralOptionsModel.Instance);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        GeneralOptionsModel.Saved -= this.GeneralPageOnSaved;
        this._editorBarControl.Dispose();
    }

    /// <summary>
    /// Gets the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" /> with the given
    /// <paramref name="marginName" />.
    /// </summary>
    /// <param name="marginName">The name of the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" />.</param>
    /// <returns>
    /// The <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" /> named <paramref name="marginName" />, or
    /// null if no match is found.
    /// </returns>
    /// <remarks>
    /// A margin returns itself if it is passed its own name. If the name does not match and it is a container margin, it
    /// forwards the call to its children. Margin name comparisons are case-insensitive.
    /// </remarks>
    public ITextViewMargin? GetTextViewMargin(string marginName)
    {
        return marginName == "EditorBar-" + this._position ? this : null;
    }

    /// <summary>
    /// Gets the size of the margin.
    /// </summary>
    /// <remarks>
    /// For a horizontal margin this is the height of the margin,
    /// since the width will be determined by the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView" />.
    /// For a vertical margin this is the width of the margin, since the height will be determined by the
    /// <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView" />.
    /// </remarks>
    public double MarginSize => this.Enabled ? this._editorBarControl.ActualHeight : 0;

    /// <summary>
    /// Determines whether the margin is enabled.
    /// </summary>
    public bool Enabled { get; private set; }

    /// <summary>
    /// Gets the <see cref="T:System.Windows.FrameworkElement" /> that renders the margin.
    /// </summary>
    public FrameworkElement VisualElement => this._editorBarControl;

    private void GeneralPageOnSaved(GeneralOptionsModel generalOptionsModel)
    {
        this.Enabled = generalOptionsModel.Enabled && generalOptionsModel.BarPosition == this._position;
        this._editorBarControl.Visibility = this.Enabled ? Visibility.Visible : Visibility.Collapsed;
    }
}